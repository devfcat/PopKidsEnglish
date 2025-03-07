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
        Screen.orientation = ScreenOrientation.LandscapeRight;

        // AI 초기 설정
        AI_Manager.Instance.Init_AI();

        On_Panel();
    }

    // 메인 씬에서의 패널 관리 메서드
    public void On_Panel()
    {
        eState state = GameManager.Instance.m_state;
        Debug.Log("현재 state: " + state);
        
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
            default:
                Debug.Log("DrawManager On_Panel 예외발생");
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
}
