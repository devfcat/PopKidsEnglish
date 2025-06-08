using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

[Serializable]
public class WordBook
{
    [SerializeField] public List<Word> wordFile;
}

[Serializable]
public class Word
{
    [SerializeField] public string english;
    [SerializeField] public string korean;
}

[Serializable]
public class Version
{
    [SerializeField] public string version;
}

/// <summary>
/// 단어장 관리 및 관련 기능
/// </summary>
public class WordManager : MonoBehaviour
{
    public bool isLoading;
    public string m_version;
    public bool isLatest;

    [Header("다운로드 서버 URL")]
    public string __url;
    public string m_section; // 현재 단어 분류
    [SerializeField] public List<Word> wordList; // 불러온 단어 리스트
    [SerializeField] public string m_english; // 현재 선택된 영단어
    [SerializeField] public string m_korean; // 현재 선택된 영단어의 뜻
    [SerializeField] public int id; // 현재 선택된 단어의 아이디

    // WordManager 인스턴스화 싱글톤 패턴
    private static WordManager _instance;
    public static WordManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(WordManager)) as WordManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        m_english = "";
        m_korean = "";
        m_section = "";

        StartCoroutine(CheckVersion()); // 버전체크
    }

    // 앱 시작 시 단어장 파일의 버전을 확인하고 버전이 다를 시 업데이트를 진행하도록 flow 진행
    // 업데이트 시 패널을 띄우고 잠시 대기하도록 처리할 것.
    public IEnumerator CheckVersion()
    {
        isLoading = true;

        // 버전 체크 파일이 있는지 확인
        string filePath = Path.Combine(Application.persistentDataPath, "version.json");

        if (File.Exists(filePath))
        {
            string jsonContents = File.ReadAllText(filePath);
            Version content = JsonUtility.FromJson<Version>(jsonContents);
            m_version = content.version; // 현재 파일의 버전을 체크한다
        }

        yield return StartCoroutine(DownloadVersionFile(filePath)); // 서버로부터 버전 파일을 다운받는다

        string dowloadContents = File.ReadAllText(filePath);
        Version dcontent = JsonUtility.FromJson<Version>(dowloadContents);
        string latest_version = dcontent.version;

        // 처음 앱을 다운받을 경우에는 버전에 대한 기록이 없음 or 버전 다를 경우
        if (m_version == "" || m_version == null || m_version != latest_version)
        {
            isLatest = false;

        }
        else // 버전이 최신인 경우
        {
            isLatest = true;
        }

        isLoading = false;
    }

    // 버전 파일을 다운로드
    public IEnumerator DownloadVersionFile(string filePath)
    {
        // 다운로드할 url
        string url = __url + "version.json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        // 에러 발생 시 
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            isLoading = false;
            Debug.LogError("File download error: " + request.error);

            GameManager.Instance.ErrorCode = 100;
        }
        else // 정상 작동
        {
            // 다운로드된 파일을 로컬에 저장
            File.WriteAllBytes(filePath, request.downloadHandler.data);
            Debug.Log("File downloaded to: " + filePath);
        }
    }

    public IEnumerator DownloadFile(string section)
    {
        isLoading = true;

        // 로컬 저장할 공간
        string filePath = Path.Combine(Application.persistentDataPath, "Word/" + section + ".json");

        if (!Directory.Exists(Application.persistentDataPath + "/Word"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Word");
        }

        if (File.Exists(filePath) && isLatest) // 이미 다운받은 적이 있다면 return
        {
            Set_WordBooks(filePath);
            isLoading = false;
        }
        else // 파일이 없으면
        {
            // 다운로드할 url
            string url = __url + section + ".json";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            // 에러 발생 시 
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                isLoading = false;
                Debug.LogError("File download error: " + request.error);

                GameManager.Instance.ErrorCode = 100;
            }
            else // 정상 작동
            {
                // 다운로드된 파일을 로컬에 저장
                File.WriteAllBytes(filePath, request.downloadHandler.data);
                Debug.Log("File downloaded to: " + filePath);

                Set_WordBooks(filePath);
                isLoading = false;
            }
        }
    }

    // 단어장 화면을 구성함
    void Set_WordBooks(string path)
    {
        // 다운로드한 파일을 열어보기
        string jsonContents = File.ReadAllText(path);
        WordBook content = JsonUtility.FromJson<WordBook>(jsonContents);

        // Debug.Log(jsonContents);

        wordList.Clear();
        // 단어 개수 불러옴
        int length_words = content.wordFile.Count;
        for (int i = 0; i < length_words; i++)
        {
            wordList.Add(null);
        }
        for (int i = 0; i < length_words; i++)
        {
            wordList[i] = content.wordFile[i] as Word;
        }
        WordSelect.Instance.MakeWordBook();
    }

    // 사용가능한 퀴즈 목록을 엶
    public void OpenAIQuizMenu()
    {

    }
}
