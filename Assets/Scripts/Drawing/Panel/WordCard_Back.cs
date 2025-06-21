using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 뒷면이 켜질 때 영어 발음 재생
/// </summary>
public class WordCard_Back : MonoBehaviour
{
    void OnEnable()
    {
        Play_EnglishTTS.Instance.OnClick_Listen();
    }
}
