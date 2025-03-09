using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class Word_Main : MonoBehaviour
{
    [Tooltip("현재 보고 있는 단어")] public TextMeshProUGUI m_word;

    [Tooltip("이미 그렸을 때와 안그렸을 때의 버튼")]
    public GameObject btn_draw_new;
    public GameObject btn_reDraw;

    void OnEnable()
    {
        m_word.text = WordManager.Instance.m_english;
        Get_isLearned();
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

    // 이미 학습한 적이 있는가
    public void Get_isLearned()
    {
        string filepath = Application.persistentDataPath + "/" + WordManager.Instance.m_section + "_" + WordManager.Instance.m_english + ".png";

        FileInfo fileInfo = new FileInfo(filepath); 

        if (fileInfo.Exists) // 그린 적 있음
        { 
            btn_draw_new.SetActive(false);
            btn_reDraw.SetActive(true);
        }
        else // 그린 적이 없음
        {
            btn_draw_new.SetActive(true);
            btn_reDraw.SetActive(false);
        }
    }

}
