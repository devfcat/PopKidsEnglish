using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메인 씬에서의 주요 기능을 담당하는 스크립트
/// </summary>

public class MainManager : MonoBehaviour
{
    [Tooltip("패널들")] public List<GameObject> panels;
    [Tooltip("헤더")] public List<GameObject> headers;
    public GameObject icon;
    public GameObject btn_back;

    // MainManager 인스턴스화 싱글톤 패턴
    private static MainManager _instance;
    public static MainManager Instance
    {
        get {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(MainManager)) as MainManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    // 기본 세팅이 들어가 있음
    void OnEnable()
    {
        SoundManager.Instance.PlayBGM(BGM.Main);
        Screen.orientation = ScreenOrientation.Portrait;

        On_Panel();
    }

    // 메인 씬에서의 패널 관리 메서드
    public void On_Panel()
    {
        eState state = GameManager.Instance.m_state;
        Debug.Log("현재 state: " + state);
        
        SetIconOrBack(state);
        switch(state)
        {
            case eState.Main_Introduce:
                SetPanels(0);
                break;
            case eState.Main_Terms:
                SetPanels(1);
                break;
            case eState.Main_Menu:
                SetPanels(2);
                break;
            case eState.Main_WordBook:
                SetPanels(3);
                break;
            case eState.Main_QuizMenu:
                SetPanels(4);
                break;
            case eState.MyDrawing_Menu:
                SetPanels(5);
                break;
            default:
                Debug.Log("MainManager On_Panel 예외발생");
                break;
        }
    }

    // 해당 패널만 키고 나머지는 끄는 메서드
    private void SetPanels(int num)
    {
        for (int i=0; i < panels.Count; i++)
        {
            if (num == i)
            {
                panels[i].SetActive(true);
            }
            else
            {
                panels[i].SetActive(false);
            }
        }

        SetHeader(num);
    }

    // 헤더의 텍스트 화면별 적용
    private void SetHeader(int num)
    {
        for (int i=0; i < headers.Count; i++)
        {
            if (num == i)
            {
                headers[i].SetActive(true);
            }
            else
            {
                headers[i].SetActive(false);
            }
        }

    }

    private void SetIconOrBack(eState state)
    {
        if (state == eState.Main_Menu)
        {
            icon.SetActive(true);
            btn_back.SetActive(false);
        }
        else
        {
            icon.SetActive(false);
            btn_back.SetActive(true);
        }
    }

    public void GoHome()
    {
        GameManager.Instance.SetState(eState.Main_Menu);
    }
}
