using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Draw 씬에서의 주요 기능을 담당하는 스크립트
/// </summary>

public class DrawManager : MonoBehaviour
{
    [Tooltip("패널들")] public List<GameObject> panels;
    [Tooltip("헤더")] public List<GameObject> headers;

    [Header("AI 단어 설명칸들")] public List<TextMeshProUGUI> ai_infos;
    [Tooltip("AI 설명")] public string info = "";

    [Header("로딩 처리")]
    public GameObject panel_Loading;

    [Header("그림 결과용 뒤로가기 버튼")]
    public GameObject btn_back;
    public GameObject btn_back_result;
    public GameObject btn_back_main;

    public GameObject popup_saveDone; // 그림 저장 완료 팝업

    [SerializeField] private int isAudioActive; // 소리 켜짐(0) 꺼짐(1)
    public List<GameObject> btn_audios; // 오디오버튼

    // DrawManager 인스턴스화 싱글톤 패턴
    private static DrawManager _instance;
    public static DrawManager Instance
    {
        get {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(DrawManager)) as DrawManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    // 추가 설명을 가져올때 로딩창을 띄움
    void Update()
    {
        bool isLoading = AI_Manager.Instance.isLoading;
        panel_Loading.SetActive(isLoading);
    }

    // 기본 세팅이 들어가 있음
    void OnEnable()
    {
        SoundManager.Instance.PlayBGM(BGM.DrawAndWord);

        // AI 초기 설정
        AI_Manager.Instance.Init_AI();

        Get_Audio();
        On_Panel();
    }

    // 메인 씬에서의 패널 관리 메서드
    public void On_Panel()
    {
        eState state = GameManager.Instance.m_state;
        Debug.Log("현재 state: " + state);

        SetBackBTN();
        
        switch(state)
        {
            case eState.Word_Main:
                SetPanels(0);
                SetHeader(2);
                AI_Manager.Instance.Get_Info();
                break;
            case eState.Word_JustAnswer:
                SetPanels(1);
                SetHeader(1);
                break;
            case eState.Word_Draw:
                SetPanels(2);
                SetHeader(2);
                Set_AIText();
                break;
            case eState.Word_DrawResult:
                SetPanels(3);
                SetHeader(1);
                break;
            case eState.Draw:
                SetPanels(4);
                SetHeader(0);
                break;
            case eState.Draw_Result:
                SetPanels(5);
                SetHeader(0);
                break;
            case eState.Draw_Intro:
                SetPanels(6);
                SetHeader(0);
                break;
            default:
                Debug.Log("DrawManager On_Panel 예외발생");
                break;
        }
    }

    public void SetBackBTN()
    {
        eState state = GameManager.Instance.m_state;

        switch(state)
        {
            case eState.Word_DrawResult:
                btn_back_main.SetActive(false);
                btn_back_result.SetActive(true); // 이 화면에서만 뒤로가기 버튼 생김 다름
                btn_back.SetActive(false);
                break;
            default:
                btn_back_main.SetActive(false);
                btn_back_result.SetActive(false);
                btn_back.SetActive(true);
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
    }

    // 헤더의 텍스트 화면별 적용 (0이면 끔, 1이면 발음듣기, 2면 추가설명까지 2개)
    private void SetHeader(int num)
    {
        for(int i = 0; i < headers.Count; i++)
        {
            headers[i].SetActive(false);
        }

        if (num == 0)
        {
            return;
        }
        else if (num == 1)
        {
            headers[0].SetActive(true);
            headers[1].SetActive(true);
        }
        else
        {
            headers[0].SetActive(true);
            headers[1].SetActive(true);
            headers[2].SetActive(true);
        }
    }

    // 뒤로가기
    public void Onclick_Back()
    {
        eState state = GameManager.Instance.m_state;

        switch(state)
        {
            case eState.Word_Main:
                GameManager.Instance.SetState(eState.Main_WordBook);
                break;
            case eState.Word_JustAnswer:
                GameManager.Instance.SetState(eState.Word_Main);
                break;
            case eState.Word_Draw:
                GameManager.Instance.SetState(eState.Word_Main);
                break;
            case eState.Word_DrawResult:
                GameManager.Instance.SetState(eState.Main_WordBook);
                break;
            case eState.Draw:
                GameManager.Instance.SetState(eState.Draw_Intro);
                break;
            case eState.Draw_Result:
                GameManager.Instance.SetState(eState.Draw);
                break;
            case eState.Draw_Intro:
                GameManager.Instance.SetState(eState.Main_Menu);
                break;
            default:
                GameManager.Instance.SetState(eState.Main_WordBook);
                break;
        }
    }

    // 추가설명요청
    public void Onclick_AddInfo()
    {
        SoundManager.Instance.PlaySFX(SFX.pretty);
        AI_Manager.Instance.Get_AddInfo();
    }

    // AI_Manager에서 정보 요청후 해당 비동기 메서드에서 실행함
    public void Set_AIText()
    {
        eState state = GameManager.Instance.m_state;

        switch(state)
        {
            case eState.Word_Main:
                ai_infos[0].text = info;
                break;
            case eState.Word_Draw:
                ai_infos[1].text = info;
                break;
            default:
                break;
        }
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
