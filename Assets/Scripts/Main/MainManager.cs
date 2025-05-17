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
    public GameObject panel_Loading;
    [SerializeField] private int isAudioActive; // 소리 켜짐(0) 꺼짐(1)
    public List<GameObject> btn_audios; // 오디오버튼

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

    void Update()
    {
        bool isLoading = WordManager.Instance.isLoading;
        panel_Loading.SetActive(isLoading);
    }

    // 기본 세팅이 들어가 있음
    void OnEnable()
    {
        SoundManager.Instance.PlayBGM(BGM.Main);
        Screen.orientation = ScreenOrientation.Portrait;
        Get_Audio();
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
            case eState.MyDrawing_List:
                SetPanels(5);
                break;
            case eState.MyDrawing_Menu:
                SetPanels(4);
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

                Vector2 m_position = panels[i].transform.parent.GetComponent<RectTransform>().anchoredPosition;
                // 스크롤뷰 초기화
                panels[i].transform.parent.GetComponent<RectTransform>().anchoredPosition
                = new Vector2(m_position.x, 0f);
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
            icon.SetActive(true);
            btn_back.SetActive(true);
        }
    }

    public void GoHome()
    {
        GameManager.Instance.SetState(eState.Main_Menu);
    }

    // 소리 버튼 초기 설정 (시작 시 한번만 실행됨)
    public void Get_Audio()
    {
        isAudioActive = PlayerPrefs.GetInt("isAudioActive");
        if (isAudioActive == 0) // 켜짐
        {
            SoundManager.Instance.Set_Volume(true);
            btn_audios[0].SetActive(true);
            btn_audios[1].SetActive(false);
        } 
        else // 꺼짐
        {
            SoundManager.Instance.Set_Volume(false);
            btn_audios[0].SetActive(false);
            btn_audios[1].SetActive(true);
        }
    }

    // 실제 적용되는 메서드
    public void Set_Audio()
    {
        if (isAudioActive == 0) // 켜짐
        {
            isAudioActive = 1; // 꺼지게 만듦
            SoundManager.Instance.Set_Volume(false);
            btn_audios[0].SetActive(false);
            btn_audios[1].SetActive(true);
        } 
        else // 꺼짐
        {
            isAudioActive = 0; // 켜지게 만듦
            SoundManager.Instance.Set_Volume(true);
            btn_audios[0].SetActive(true);
            btn_audios[1].SetActive(false);
        }
        PlayerPrefs.SetInt("isAudioActive", isAudioActive);
    }
}
