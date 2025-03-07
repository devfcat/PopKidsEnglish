using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Word_JustAnswer : MonoBehaviour
{
    public TextMeshProUGUI word;
    void OnEnable()
    {
        word.text = WordManager.Instance.m_english + " / " + WordManager.Instance.m_korean;
    }
}
