using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 앱 전체 상태관리를 위한 총괄 매니저 스크립트
/// 인스턴스화 되어 다른 매니저에서 메서드 호출가능
/// </summary>

// 화면 상태 종류 (State)
 public enum eState
 {
    Splash = 0, // 스플래시 화면
    Main_Introduce, // 사용법 설명 화면
    Main_Terms, // 이용약관 처음볼 때

    /// <summary>
    /// 메인에 소속된 화면
    /// </summary>

    Main_Menu,
    Main_WordBook, // 단어장 목록 조회

    /// <summary>
    /// 단어 보기, 그리기
    /// </summary>
    
    Word_Main,
    Word_JustAnswer,
    Word_Draw, // (그림 / 단어 그리기)
    Word_DrawResult,
    AR,

    /// <summary>
    /// AI 생성 퀴즈
    /// </summary>
    
    Quiz_Main,

    // 추가 필요

    /// <summary>
    /// 그림판 모드
    /// </summary>
    Draw_Intro, // Draw 시작 전
    Draw, // Draw 씬
    Draw_Result, // Draw 씬

    /// <summary>
    /// 내 단어장 보기
    /// </summary>
    
    MyDrawing_List, // 내가 만든 단어장 화면
    MyDrawing_Menu, // 내가 만든 단어장 주제별 목록조회
    MyDrawing_View, // 내 단어장 보기 (개별)
    None
 }

public class GameManager : MonoBehaviour
{
    // GameManager의 인스턴스화 싱글톤 패턴
    private static GameManager _instance;
    public static GameManager Instance
    {
        get {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // 다른 씬에서도 사용 가능
    }

    [Header("상태 관리")]
    public eState prev_state; // 이전 화면
    public eState m_state; // 사용자의 현 화면 상태

    public int ErrorCode = 0; // 에러 발생 시 코드

    [Header("팝업")]
    public GameObject popUp_Exit;
    public GameObject popUp_Error;

    public float screen_width;
    public float screen_height;

    void Start()
    {
        Init(); // 시작 시 초기화
    }

    private void Init()
    {
        screen_width = Screen.width;
        screen_height = Screen.height;
        
        SetResolution.Instance.Get_ScreenSize();
        SetResolution.Instance.Set_Panel();

        prev_state = eState.None;
        Application.targetFrameRate = 60; // 60 프레임 고정
        SetState(eState.Splash);
    }

    // 세로 모드, 가로 모드 설정 메서드
    public void SetOrientation(bool isPortrait = true)
    {
        if (isPortrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    // 상태 관리 머신
    public void SetState(eState state)
    {
        StartCoroutine(Change_State(state));
    }

    // 자연스러운 화면 전환을 위해 딜레이를 줌
    IEnumerator Change_State(eState state)
    {
        prev_state = m_state;

        m_state = state;

        string m_scene = SceneManager.GetActiveScene().name;

        yield return new WaitForSeconds(0.2f);

        if (state == eState.Splash)
        {

        }
        else
        {
            LoadStateScene(m_scene, state);
        }
        /*
        switch(state)
        {
            case eState.Splash:
                break;
            case eState.Main_Menu:
                LoadStateScene(m_scene, state);
                break;
            case eState.Main_WordBook:
                LoadStateScene(m_scene, state);
                break;
            case eState.Main_QuizMenu:
                LoadStateScene(m_scene, state);
                break;
            case eState.MyDrawing_Menu:
                LoadStateScene(m_scene, state);
                break;
            case eState.Word_Main:
                LoadStateScene(m_scene, state);
                break;
            case eState.Word_JustAnswer:
                LoadStateScene(m_scene, state);
                break;
            case eState.Word_Draw:
                LoadStateScene(m_scene, state);
                break;
            case eState.Word_DrawResult:
                LoadStateScene(m_scene, state);
                break;
            case eState.Draw:
                LoadStateScene(m_scene, state);
                break;
            case eState.Draw_Result:
                LoadStateScene(m_scene, state);
                break;
            case eState.Draw_Intro:
                LoadStateScene(m_scene, state);
                break;
            default:
                LoadStateScene(m_scene, eState.Main_Menu);
                break;
        }
        */
    }


    // 이전 화면과 씬이 다르면 씬을 로드하는 메서드
    public void LoadStateScene(string m_sceneName, eState next)
    {
        string n_sceneName = "";
        n_sceneName = next.ToString();

        // 이전 씬이 스플래시였다면 어떤 씬으로 가기를 호출했던지 메인 씬 불러옴
        if (m_sceneName == "Splash")
        {
            SceneManager.LoadScene("Main");
            return;
        }
        
        if (next == eState.None || next == eState.Splash)
        {
            Debug.Log("오류 발생");
            return;
        }

        if (next == eState.AR)
        {
            n_sceneName = "AR";
        }
        else if (n_sceneName.Contains("MyDrawing_") || n_sceneName.Contains("Main_"))
        {
            if (n_sceneName == "MyDrawing_View") n_sceneName = "Draw";
            else n_sceneName = "Main";
        }
        else if (n_sceneName.Contains("Quiz_"))
        {
            n_sceneName = "Quiz";
        }
        else if (n_sceneName.Contains("Draw_") || n_sceneName.Contains("Word_"))
        {
            n_sceneName = "Draw";
        }

        if (m_sceneName != n_sceneName)
        {
            SceneManager.LoadScene(n_sceneName);
        }
        else // 이전 화면과 바꿀 화면의 씬이 동일할 경우
        {
            if (n_sceneName == "Main")
            {
                MainManager.Instance.On_Panel();
            }
            else if (n_sceneName == "Draw")
            {
                DrawManager.Instance.On_Panel();
            }
            else if (n_sceneName == "Quiz")
            {
                
            }

        }
    }

    void Update()
    {
        // 뒤로가기 누르면 나가기 or 나가기 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPopupExit();
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ErrorCode = -1;
        }

        if (ErrorCode == -1)
        {
            Open_ErrorPopup("인터넷 연결을 확인해주세요.");
        }
        else if (ErrorCode == 100)
        {
            Open_ErrorPopup("정보를 불러올 수 없습니다.\n문제가 계속되면 devfcat@gmail.com으로 문의바랍니다.");
        }
        else if (ErrorCode == 200)
        {
            Open_ErrorPopup("현재 서비스를 이용하실 수 없습니다.\n문제가 계속되면 devfcat@gmail.com으로 문의바랍니다.");
        }
        else if (ErrorCode == 201)
        {
            Open_ErrorPopup("파일을 불러올 수 없습니다.\n문제가 계속되면 devfcat@gmail.com으로 문의바랍니다.");
        }
    }

    void Open_ErrorPopup(string msg)
    {
        popUp_Error.SetActive(true);
        popUp_Error.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = msg;
        ErrorCode = 0;
        SetState(eState.Main_Menu);
    }

    // 나가기 팝업 제어
    public void SetPopupExit()
    {
        if(popUp_Exit.activeSelf)
        {
            popUp_Exit.SetActive(false);
        }
        else
        {
            popUp_Exit.SetActive(true);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

}
