using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Word_Main : MonoBehaviour
{
    [Tooltip("현재 보고 있는 단어")] public TextMeshProUGUI m_word;

    void OnEnable()
    {
        m_word.text = WordManager.Instance.m_english;
    }

    // 그냥 정답 볼래요
    public void OnClick_ViewAnswer()
    {
        GameManager.Instance.SetState(eState.Word_JustAnswer);
    }

    public void OnClick_GoPaint()
    {
        GameManager.Instance.SetState(eState.Word_Draw);
    }

}
