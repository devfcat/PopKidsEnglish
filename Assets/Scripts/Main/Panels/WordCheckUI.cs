using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class WordCheckUI : MonoBehaviour
{
    public GameObject ball_red;
    public GameObject ball_green;

    public Transform basket; // 공을 담을 바구니 트랜스폼

    public int words_learned;
    private int words_all = 184; // 총 단어 수(2025년 6월 21일 기준) - Basic 단어장들은 제외됨

    public TextMeshProUGUI tmp_learned; // 학습한 단어 개수 표시

    [Header("생성된 공 인스턴스 관리")]
    [SerializeField] private List<GameObject> ballInstances = new List<GameObject>(); // 생성된 공 인스턴스들을 관리

    void OnEnable()
    {
        CountLearnedWords();
        StartCoroutine(SetUI());
    }

    void OnDisable()
    {
        // 생성된 모든 공 인스턴스들 삭제
        ClearBallInstances();
    }

    // 학습한 단어 개수를 세는 메서드
    void CountLearnedWords()
    {
        try
        {
            string directoryPath = Application.persistentDataPath;
            
            if (Directory.Exists(directoryPath))
            {
                // PNG 파일들만 찾기
                string[] pngFiles = Directory.GetFiles(directoryPath, "*.png");
                words_learned = pngFiles.Length;
            }
            else
            {
                words_learned = 0;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("단어 개수 세기 오류: " + e.Message);
            words_learned = 0;
        }
    }

    // 생성된 공 인스턴스들을 모두 삭제
    void ClearBallInstances()
    {
        foreach (GameObject ball in ballInstances)
        {
            if (ball != null)
            {
                Destroy(ball);
            }
        }
        ballInstances.Clear();
    }

    IEnumerator SetUI()
    {
        // 이전에 생성된 공들 정리
        ClearBallInstances();

        tmp_learned.text = "전체 " + words_all.ToString() + "개 중 " + words_learned.ToString() + "개";
    
        // 학습한 단어 개수에 따라 전체 공 16개 중 몇 개가 초록색으로 변할지(학습 비율에 따름) 적용
        int green_count = (int)((words_learned / words_all)*16);
        int red_count = 16 - green_count;

        // 공을 바구니에 인스턴스로 각 개수마다 0.1초 간격으로 생성한다
        for (int i = 0; i < green_count; i++)
        {
            GameObject ball = Instantiate(ball_green, basket);
            // 바구니 x좌표에서 ±8f 범위로 랜덤한 x좌표 설정
            float randomX = basket.position.x + Random.Range(-8f, 8f);
            ball.transform.position = new Vector3(randomX, ball.transform.position.y, ball.transform.position.z);
            
            // 생성된 공을 리스트에 추가
            ballInstances.Add(ball);
            
            yield return new WaitForSeconds(0.2f);
        }
        for (int i = 0; i < red_count; i++)
        {
            GameObject ball = Instantiate(ball_red, basket);
            // 바구니 x좌표에서 ±8f 범위로 랜덤한 x좌표 설정
            float randomX = basket.position.x + Random.Range(-8f, 8f);
            ball.transform.position = new Vector3(randomX, ball.transform.position.y, ball.transform.position.z);
            
            // 생성된 공을 리스트에 추가
            ballInstances.Add(ball);
            
            yield return new WaitForSeconds(0.2f);
        }
    }
}
