using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    

public class QuizManager : MonoBehaviour
{
    [Tooltip("패널들")] public List<GameObject> panels;
    public TextMeshProUGUI word_theme_tmp;
    public GameObject theme_header;

    [Header("로딩 처리")]
    public GameObject panel_Loading;

    [Header("그림 결과용 뒤로가기 버튼")]
    public GameObject btn_back;
    [SerializeField] private int isAudioActive; // 소리 켜짐(0) 꺼짐(1)
    public List<GameObject> btn_audios; // 오디오버튼

    public int score; // 퀴즈 점수 (최대 5점)

    // QuizManager 인스턴스화 싱글톤 패턴
    private static QuizManager _instance;
    public static QuizManager Instance
    {
        get {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(QuizManager)) as QuizManager;

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
        score = 0;

        SoundManager.Instance.PlayBGM(BGM.Quiz);

        // AI 초기 설정
        AI_Manager.Instance.Init_AI();

        word_theme_tmp.text = "주제 : \n" + WordManager.Instance.m_section;

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
            case eState.Quiz_Intro:
                SetPanels(0);
                theme_header.SetActive(true);
                break;
            case eState.Quiz_Main:
                SetPanels(1);
                theme_header.SetActive(false);
                break;
            case eState.Quiz_Result:
                SetPanels(2);
                theme_header.SetActive(true);
                break;
            default:
                Debug.Log("QuizManager On_Panel 예외발생");
                break;
        }
    }

    public void SetBackBTN()
    {
        btn_back.SetActive(true);
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

    // 뒤로가기
    public void Onclick_Back()
    {
        eState state = GameManager.Instance.m_state;

        switch(state)
        {
            case eState.Quiz_Intro:
                GameManager.Instance.SetState(eState.Main_WordBook);
                break;
            case eState.Quiz_Main:
                GameManager.Instance.SetState(eState.Quiz_Intro);
                break;
            case eState.Quiz_Result:
                GameManager.Instance.SetState(eState.Quiz_Intro);
                break;
            default:
                GameManager.Instance.SetState(eState.Main_WordBook);
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

    public void SetAnswerText(GameObject popup, QuizQuestion currentQuestion)
    {
        if (popup == null) return;
        // 두 번째 자식이 TextMeshProUGUI라고 가정
        var text = popup.transform.GetChild(0).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = $"정답 : {currentQuestion.correctWord}";
        }
    }
}
