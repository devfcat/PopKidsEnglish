using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// AR 씬에서 단어 카드에 정보 및 내가 만든 단어를 넣는 스크립트
/// </summary>
public class WordCard3D : MonoBehaviour
{
    public static WordCard3D Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI english_tmp; // 카드 앞면의 영단어 표기
    [SerializeField] private TextMeshProUGUI textBack_tmp; // 카드 뒷면의 한글 및 영단어 표기
    [SerializeField] public RawImage card_image; // 카드 이미지

    public bool isFront = true; // 현재 앞면인지 여부
    private Animator animator;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        isFront = true;
    }

    void OnEnable()
    {
        string filepath = Application.persistentDataPath + "/" + WordManager.Instance.m_section + "_" + WordManager.Instance.id + ".png";
        SetCardUI(WordManager.Instance.m_english, WordManager.Instance.m_korean, filepath);
    }

    public void SetCardUI(string english, string korean, string imagePath)
    {
        english_tmp.text = english;
        textBack_tmp.text = english + "\n" + korean;

        // 파일에서 이미지 로드
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageData);
            card_image.texture = tex;
        }
        else
        {
            Debug.LogWarning("이미지 파일이 존재하지 않습니다: " + imagePath);
            card_image.texture = null;
        }

        CardSetManager.Instance.card = this.gameObject;
    }

    public void FlipCard()
    {
        if (isFront)
        {
            animator.SetTrigger("ToBack");
        }
        else if (!isFront)
        {
            animator.SetTrigger("ToFront");
        }

        isFront = !isFront;
    }
}
