using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_word : MonoBehaviour
{
    public int id; // 단어장에서 몇 번째인가

    public void OnClick()
    {
        WordManager.Instance.m_english = WordManager.Instance.wordList[id].english;
        WordManager.Instance.m_korean = WordManager.Instance.wordList[id].korean;

        GameManager.Instance.SetState(eState.Word_Main);
    }
}
