using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // 단어 분류 버튼
    public void Open_WordSelect(string section)
    {
        WordManager.Instance.m_section = section;
        GameManager.Instance.SetState(eState.Main_WordBook);
    }

    // 퀴즈 메뉴 들어가기
    public void Open_QuizMenu()
    {
        GameManager.Instance.SetState(eState.Main_QuizMenu);
    }
}
