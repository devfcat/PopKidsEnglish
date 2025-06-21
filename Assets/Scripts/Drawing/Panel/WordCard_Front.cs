using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordCard_Front : MonoBehaviour
{
    void Start()
    {
        Play_KoreanTTS.Instance.lastComment = "";
    }

    void OnEnable()
    {
        Play(DrawManager.Instance.info);
    }

    public void Play(string m_text)
    {
        Play_KoreanTTS.Instance.Play_Info(m_text);
    }
}
