using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Word_JustAnswer : MonoBehaviour
{
    public TextMeshProUGUI word;
    public GameObject btn_next;

    void OnEnable()
    {
        word.text = WordManager.Instance.m_english + " / " + WordManager.Instance.m_korean;
        Set_NextButton();
    }

    // 단어장에서 이 단어가 마지막이 아니어야 다음으로 버튼이 나옴
    public void Set_NextButton()
    {
        int m_id = WordManager.Instance.id;
        int word_length = WordManager.Instance.wordList.Count;

        if (m_id - 1 >= word_length)
        {
            btn_next.SetActive(false);
        }
        else
        {
            btn_next.SetActive(true);
        }
    }  

    // 다음 단어 학습 가능
    public void Onclick_Next()
    {
        int m_id = WordManager.Instance.id;
        WordManager.Instance.id = m_id + 1;
        m_id = WordManager.Instance.id;

        WordManager.Instance.m_english = WordManager.Instance.wordList[m_id].english;
        WordManager.Instance.m_korean = WordManager.Instance.wordList[m_id].korean;

        GameManager.Instance.SetState(eState.Word_Main);
    }
}
