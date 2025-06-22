using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnSelect : MonoBehaviour
{
    public void OnClick(string section)
    {
        LearnSelect_Manager.Instance.m_selected_section = section;
        WordManager.Instance.m_section = LearnSelect_Manager.Instance.m_selected_section;
        GameManager.Instance.SetState(eState.MyDrawing_Menu);
    }
}
