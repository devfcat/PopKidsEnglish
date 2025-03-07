using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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

/// <summary>
/// 단어장 관리 및 관련 기능
/// </summary>
public class WordManager : MonoBehaviour
{
    public string m_section; // 현재 단어 분류
    [SerializeField] public List<Word> wordList; // 불러온 단어 리스트
    [SerializeField] public string m_english; // 현재 선택된 영단어
    [SerializeField] public string m_korean; // 현재 선택된 영단어의 뜻

    // WordManager 인스턴스화 싱글톤 패턴
    private static WordManager _instance;
    public static WordManager Instance
    {
        get {
            if(!_instance)
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
    }

    // 단어장에서 한 분류를 클릭하면 해당 분류 단어장을 엶 (단어 목록 보기)
    public IEnumerator Coroutine_OpenWordMenu(string section)
    {
        string filepath = "Assets/Words/" + section + ".json";

        string jsonContents = File.ReadAllText(filepath);

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
            wordList[i] =  content.wordFile[i] as Word;
        }

        WordSelect.Instance.MakeWordBook();

        yield return null;
    }

    // 사용가능한 퀴즈 목록을 엶
    public void OpenAIQuizMenu()
    {

    }
}
