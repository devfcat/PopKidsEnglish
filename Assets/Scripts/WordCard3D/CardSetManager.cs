using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CardSetManager : MonoBehaviour
{
    public GameObject card;
    public static CardSetManager Instance { get; private set; }
    public GameObject cardPrefab;
    public ARPlaneManager planeManager;
    public Camera arCamera;

    private bool isCardPlaced = false;

    [Header("배치 거리 조정")]
    public float distanceFromCamera = 0.5f;  // 카메라 기준 앞쪽 거리

    [Header("평면 인식 대기 시간(초)")]
    private float waitTimeForPlane = 5.0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void OnEnable()
    {
        isCardPlaced = false;
        planeManager.planesChanged += OnPlanesChanged;
        StartCoroutine(PlaceCardIfNoPlane());
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
        StopAllCoroutines();
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count == 0) return;

        ARPlane plane = args.added[0];

        if (!isCardPlaced || card == null)
        {
            // 카드가 없으면 새로 생성
            PlaceCardOnPlane(plane);
        }
        else
        {
            // 카드가 이미 있으면 위치/회전만 평면 위로 옮김
            Vector3 cameraForward = arCamera.transform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            Vector3 spawnPosition = arCamera.transform.position + cameraForward * distanceFromCamera;
            spawnPosition.y = plane.transform.position.y;

            Vector3 directionToCamera = arCamera.transform.position - spawnPosition;
            directionToCamera.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(directionToCamera);

            card.transform.position = spawnPosition;
            card.transform.rotation = rotation;
        }

        isCardPlaced = true;
    }

    private void PlaceCardOnPlane(ARPlane plane)
    {
        Vector3 cameraForward = arCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 spawnPosition = arCamera.transform.position + cameraForward * distanceFromCamera;
        spawnPosition.y = plane.transform.position.y;

        Vector3 directionToCamera = arCamera.transform.position - spawnPosition;
        directionToCamera.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(directionToCamera);

        GameObject card = Instantiate(cardPrefab, spawnPosition, rotation);
        card.SetActive(true);

        isCardPlaced = true;
    }

    // 평면이 일정 시간 동안 감지되지 않으면 카메라 앞에 카드를 생성하는 코루틴
    private IEnumerator PlaceCardIfNoPlane()
    {
        // 평면 인식을 기다리는 시간만큼 대기
        yield return new WaitForSeconds(waitTimeForPlane);

        // 아직 카드가 생성되지 않았다면
        if (!isCardPlaced)
        {
            // 카메라 앞 방향 벡터 계산 (y값은 0으로 하여 수평 방향만 사용)
            Vector3 cameraForward = arCamera.transform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            // 카메라 앞쪽 일정 거리 위치 계산 (y값은 카메라와 동일)
            Vector3 spawnPosition = arCamera.transform.position + cameraForward * distanceFromCamera;
            spawnPosition.y = arCamera.transform.position.y;

            // z값은 -7.5f로 설정
            spawnPosition.z = -7.5f;

            // 카메라를 바라보는 방향으로 회전값 계산
            Vector3 directionToCamera = arCamera.transform.position - spawnPosition;
            directionToCamera.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(directionToCamera);

            // 카드 프리팹 생성 및 활성화
            GameObject card = Instantiate(cardPrefab, spawnPosition, rotation);
            card.SetActive(true);

            isCardPlaced = true;
        }
    }

    // 카드 클릭 처리 (카드 오브젝트를 인자로 받음)
    public void OnClick_Card()
    {
        if (card == null) return;

        var cardScript = card.GetComponent<WordCard3D>();
        if (cardScript == null) return;

        if (cardScript.isFront)
        {
            Play_KoreanTTS.Instance.Play_Info(WordManager.Instance.m_korean);
        }
        else
        {
            Play_EnglishTTS.Instance.OnClick_Listen();
        }

        cardScript.FlipCard();
    }
}
