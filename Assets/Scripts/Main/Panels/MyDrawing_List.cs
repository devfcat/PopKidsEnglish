using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

/// <summary>
/// 내가 그린 단어 그림들을 목록으로 보여주는 스크립트
/// </summary>
public class MyDrawing_List : MonoBehaviour
{
    [Header("UI 구성 요소")]
    public GameObject btn_word; // 단어 버튼 프리팹
    public GameObject Screen; // 단어 목록 스크린
    public GameObject title; // 단어 분류를 알려주는 타이틀

    [Header("관리용 리스트")]
    [SerializeField] private List<GameObject> instanceBoxes = new List<GameObject>(); // 인스턴스 박스들을 관리하기 위한 변수
    [SerializeField] private List<string> learnedWordIds = new List<string>(); // 학습한 단어 ID 리스트

    void OnEnable()
    {
        // 단어장 주제를 알려줌
        title.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = WordManager.Instance.m_section;
        
        // 학습한 단어 목록 구성
        StartCoroutine(BuildLearnedWordList());
    }

    // 학습한 단어 목록을 구성하는 코루틴
    IEnumerator BuildLearnedWordList()
    {
        // 위치 초기화
        Screen.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // 이전 단어장 없앰
        ClearPreviousList();

        // 현재 섹션에 해당하는 PNG 파일들 찾기
        FindLearnedWords();

        // 단어 박스들 생성
        yield return StartCoroutine(CreateWordBoxes());

        // ContentSizeExtension 적용
        this.gameObject.GetComponent<ContentSizeFitter_Extension_MyWords>().Size_Fitter();
    }

    // 이전 리스트 정리
    void ClearPreviousList()
    {
        if (instanceBoxes.Count != 0)
        {
            for (int i = 0; i < instanceBoxes.Count; i++)
            {
                if (instanceBoxes[i] != null)
                {
                    Destroy(instanceBoxes[i]);
                }
            }
        }
        instanceBoxes.Clear();
        learnedWordIds.Clear();
    }

    // 학습한 단어들 찾기
    void FindLearnedWords()
    {
        try
        {
            string directoryPath = Application.persistentDataPath;
            string section = WordManager.Instance.m_section;
            
            if (Directory.Exists(directoryPath))
            {
                // 현재 섹션에 해당하는 PNG 파일들만 찾기
                string[] pngFiles = Directory.GetFiles(directoryPath, section + "_*.png");
                
                foreach (string filePath in pngFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    // 파일명에서 ID 추출 (예: "Family_5.png" -> "5")
                    string id = fileName.Replace(section + "_", "");
                    learnedWordIds.Add(id);
                }
                
                // ID를 숫자 순으로 정렬
                learnedWordIds.Sort((a, b) => int.Parse(a).CompareTo(int.Parse(b)));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("학습한 단어 찾기 오류: " + e.Message);
        }
    }

    // 단어 박스들 생성
    IEnumerator CreateWordBoxes()
    {
        foreach (string id in learnedWordIds)
        {
            // WordManager에서 해당 ID의 단어 정보 가져오기
            int wordIndex = int.Parse(id);
            if (wordIndex < WordManager.Instance.wordList.Count)
            {
                Word word = WordManager.Instance.wordList[wordIndex];
                
                // 단어 박스 생성
                GameObject word_box = Instantiate(btn_word, Screen.transform);
                word_box.SetActive(false);
                
                // 단어 텍스트 설정
                word_box.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = word.english;
                
                // Box_word 컴포넌트에 ID 설정
                Box_word boxWord = word_box.GetComponent<Box_word>();
                if (boxWord != null)
                {
                    boxWord.id = wordIndex;
                }
                
                // Box_word_check 컴포넌트가 있다면 학습 상태 확인
                Box_word_check boxWordCheck = word_box.GetComponent<Box_word_check>();
                if (boxWordCheck != null)
                {
                    boxWordCheck.CheckLearningStatus();
                }
                
                word_box.SetActive(true);
                instanceBoxes.Add(word_box);
                
                // 0.05초 간격으로 생성하여 애니메이션 효과
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    // 외부에서 목록 새로고침을 위한 public 메서드
    public void RefreshList()
    {
        StartCoroutine(BuildLearnedWordList());
    }
} 