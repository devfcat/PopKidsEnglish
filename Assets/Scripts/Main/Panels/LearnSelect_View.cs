using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class LearnSelect_View : MonoBehaviour
{
    public TextMeshProUGUI tmp_section;
    public bool isSearch = false;
    public TMP_InputField input_search;
    public GameObject box_word;

    [Header("관리용 리스트")]
    [SerializeField] public List<GameObject> instanceBoxes = new List<GameObject>(); // 인스턴스 박스들을 관리하기 위한 변수
    [SerializeField] public List<int> learnedWordIds = new List<int>(); // 학습한 단어 ID 리스트

    private string previousSearchText = "";
    public GameObject noWordUI;

    // 싱글톤 인스턴스
    private static LearnSelect_View _instance;
    public static LearnSelect_View Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(LearnSelect_View)) as LearnSelect_View;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }
    
    void OnEnable()
    {
        tmp_section.text = LearnSelect_Manager.Instance.m_selected_section;
        Set_UI_Mode();
    }

    public void Set_UI_Mode()
    {
        if (isSearch) // 검색 중이라면 검색 결과 단어만 띄움
        {
            StartCoroutine(SearchList(input_search));
        }
        else // 검색 중이 아니라면 현재 섹션의 해당하는 내가 만든 단어장을 모두 보여줌
        {
            StartCoroutine(BuildLearnedWordList());
        }
    }

    void Update()
    {
        string currentSearchText = input_search.text;
        
        // 검색어가 변경되었을 때만 검색 실행
        if (currentSearchText != previousSearchText)
        {
            if (!string.IsNullOrEmpty(currentSearchText))
            {
                isSearch = true;
                StartCoroutine(SearchList(input_search));
            }
            else 
            {
                isSearch = false;
                // 검색창이 비어있을 때는 전체 목록으로 돌아감
                StartCoroutine(BuildLearnedWordList());
            }
            
            previousSearchText = currentSearchText;
        }
    }

    // 검색창 초기화
    public void OnClick_Delete_Search()
    {
        // previousSearchText를 먼저 업데이트하여 Update에서 중복 처리 방지
        previousSearchText = "";
        input_search.text = "";
        isSearch = false;
        StartCoroutine(BuildLearnedWordList());
    }

    // 검색창의 텍스트를 포함하는 내가 만든 단어장 파일들이 있는지 검색하고 목록으로 만듦
    IEnumerator SearchList(TMP_InputField input)
    {
        // 이전 단어장 없앰
        ClearPreviousList();

        // 검색어가 비어있으면 전체 목록 표시
        if (string.IsNullOrEmpty(input.text))
        {
            yield return StartCoroutine(BuildLearnedWordList());
            yield break;
        }

        // 검색어를 소문자로 변환하여 대소문자 구분 없이 검색
        string searchText = input.text.ToLower();

        // 단어장 불러옴
        yield return StartCoroutine(WordManager.Instance.DownloadFile(LearnSelect_Manager.Instance.m_selected_section));

        // 검색 결과를 저장할 리스트
        List<int> searchResults = new List<int>();

        // 모든 단어를 검색하여 검색어가 포함된 단어 찾기
        for (int i = 0; i < WordManager.Instance.wordList.Count; i++)
        {
            Word word = WordManager.Instance.wordList[i];
            
            // 영어 단어와 한국어 뜻에서 검색
            bool isMatch = word.english.ToLower().Contains(searchText) || 
                          word.korean.ToLower().Contains(searchText);

            if (isMatch)
            {
                // 해당 단어가 학습된 단어인지 확인 (PNG 파일 존재 여부)
                string filepath = Application.persistentDataPath + "/" + LearnSelect_Manager.Instance.m_selected_section + "_" + i + ".png";
                if (File.Exists(filepath))
                {
                    searchResults.Add(i);
                }
            }
        }

        // 검색 결과를 ID 순으로 정렬
        searchResults.Sort();

        // 검색 결과 단어 박스들 생성
        foreach (int id in searchResults)
        {
            Word word = WordManager.Instance.wordList[id];
            
            // 단어 박스 생성
            GameObject word_box = Instantiate(box_word, this.transform);
            word_box.SetActive(false);
            
            // 단어 텍스트 설정
            word_box.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = word.english;
            
            // Box_word 컴포넌트에 ID 설정
            Box_word boxWord = word_box.GetComponent<Box_word>();
            if (boxWord != null)
            {
                boxWord.id = id;
            }
            
            // Box_word_check 컴포넌트가 있다면 학습 상태 확인
            Box_word_check boxWordCheck = word_box.GetComponent<Box_word_check>();
            if (boxWordCheck != null)
            {
                boxWordCheck.CheckLearningStatus();
            }
            
            word_box.SetActive(true);
            instanceBoxes.Add(word_box);
        }

        // ContentSizeExtension 적용
        this.gameObject.GetComponent<ContentSizeFitter_Extension_MyWords>().Size_Fitter();
    }

    // 학습한 단어 목록을 구성하는 코루틴
    IEnumerator BuildLearnedWordList()
    {
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
            string section = LearnSelect_Manager.Instance.m_selected_section;
            
            if (Directory.Exists(directoryPath))
            {
                // 현재 섹션에 해당하는 PNG 파일들만 찾기
                string[] pngFiles = Directory.GetFiles(directoryPath, section + "_*.png");
                
                foreach (string filePath in pngFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    // 파일명에서 ID 추출 (예: "Family_5.png" -> 5)
                    string idString = fileName.Replace(section + "_", "");
                    if (int.TryParse(idString, out int id))
                    {
                        learnedWordIds.Add(id);
                    }
                }
                
                // ID를 숫자 순으로 정렬
                learnedWordIds.Sort();
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
        if (learnedWordIds.Count == 0)
        {
            noWordUI.SetActive(true);
            yield break;
        }
        else noWordUI.SetActive(false);

        // 단어장 불러옴
        yield return StartCoroutine(WordManager.Instance.DownloadFile(LearnSelect_Manager.Instance.m_selected_section));

        foreach (int id in learnedWordIds)
        {
            // WordManager에서 해당 ID의 단어 정보 가져오기
            if (id < WordManager.Instance.wordList.Count)
            {
                Word word = WordManager.Instance.wordList[id];
                
                // 단어 박스 생성
                GameObject word_box = Instantiate(box_word, this.transform);
                word_box.SetActive(false);
                
                // 단어 텍스트 설정
                word_box.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = word.english;
                
                // Box_word 컴포넌트에 ID 설정
                Box_word boxWord = word_box.GetComponent<Box_word>();
                if (boxWord != null)
                {
                    boxWord.id = id;
                }
                
                // Box_word_check 컴포넌트가 있다면 학습 상태 확인
                Box_word_check boxWordCheck = word_box.GetComponent<Box_word_check>();
                if (boxWordCheck != null)
                {
                    boxWordCheck.CheckLearningStatus();
                }
                
                word_box.SetActive(true);
                instanceBoxes.Add(word_box);
            }
        }
    }

    // 외부에서 목록 새로고침을 위한 public 메서드
    public void RefreshList()
    {
        StartCoroutine(BuildLearnedWordList());
    }
}
