using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 단어장 목록에서 선택한 단어를 WordManager에 저장하고 화면 전환
/// </summary>
public class Box_word : MonoBehaviour
{
    public int id; // 단어장에서 몇 번째인가

    public void OnClick()
    {
        WordManager.Instance.id = id;
        
        WordManager.Instance.m_english = WordManager.Instance.wordList[id].english;
        WordManager.Instance.m_korean = WordManager.Instance.wordList[id].korean;

        if (GameManager.Instance.m_state == eState.MyDrawing_Menu) 
        {
            WordManager.Instance.m_section = LearnSelect_Manager.Instance.m_selected_section;
            GameManager.Instance.SetState(eState.MyDrawing_View);
        }
        else GameManager.Instance.SetState(eState.Word_Main);
    }
}
