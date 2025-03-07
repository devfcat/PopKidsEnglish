using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 단어장에서 분류를 클릭하면 나오는 화면 (단어 목록)
public class WordSelect : MonoBehaviour
{
    [Tooltip("이번 단어장 목록")] [SerializeField] private List<Word> m_wordList;
    [SerializeField] private List<GameObject> instanceBoxes = new List<GameObject>(); // 인스턴스 박스들을 관리하기 위한 변수

    public GameObject btn_word; // 단어 버튼 프리팹
    public GameObject Screen; // 단어 목록 스크린
    public GameObject title; // 단어 분류를 알려주는 타이틀

    private static WordSelect _instance;
    public static WordSelect Instance
    {
        get {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(WordSelect)) as WordSelect;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    public void OnEnable()
    {
        // 단어장을 불러옴
        StartCoroutine(WordManager.Instance.Coroutine_OpenWordMenu(WordManager.Instance.m_section));
        // 단어장 주제를 알려줌
        title.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = WordManager.Instance.m_section;
    }

    // 불러온 단어 목록을 화면에 보여줌
    public void MakeWordBook()
    {
        // 위치 초기화
        Screen.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // 초기화
        m_wordList.Clear();

        // 이전 단어장 없앰
        if (instanceBoxes.Count != 0)
        {
            for (int i = 0; i < instanceBoxes.Count; i++)
            {
                Destroy(instanceBoxes[i]);
            }
        }

        // 단어 개수 불러옴
        int length_words = WordManager.Instance.wordList.Count;

        for (int i = 0; i < length_words; i++)
        {
            m_wordList.Add(null);
        }

        for (int i = 0; i < length_words; i++)
        {
            m_wordList[i] =  WordManager.Instance.wordList[i];
        }

        // 단어장 구성 
        GameObject word_box;
        for (int i = 0; i < length_words; i++)
        {
            word_box = Instantiate(btn_word, Screen.transform);
            word_box.SetActive(false);
            word_box.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_wordList[i].english;
            word_box.transform.GetComponent<Box_word>().id = i;
            word_box.SetActive(true);
            instanceBoxes.Add(word_box);
        }
    }

}
