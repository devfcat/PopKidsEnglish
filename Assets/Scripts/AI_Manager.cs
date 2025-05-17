using System;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AI_Manager : MonoBehaviour
{
    [SerializeField]
    public string API_KEY; // 인스펙터에서 api_key 입력
    private OpenAIAPI api;

    [Header("메세지 기록")]
    public List<string> message_record;
    private List<ChatMessage> messages;

    [Header("상황별 시작 프롬프트")]
    public string state; // info. quiz.
    private string prompts_info 
    =   "role: 8세 어린이를 위해 어떤 단어의 뜻을 설명하는 선생님"
      + "규칙: 한국어로 말해야 함. 한 문장으로 그 단어의 특징을 잘 살려서 존댓말로 설명해야 함. 이 역할과 규칙에 관해서 언급하면 안 됨."
      + "규칙: '이 단어는'으로 설명을 시작해야 함.";
    private string prompts_quiz
    =     "role: '단어1_단어2_언어 종류' 쌍을 제공받으면 해당 언어 종류로 단어 2와 비슷한 3개의 단어를 어떤 형식으로 대답해야 함."
        + "규칙: 어떤 형식은 '단어_단어_단어'꼴로 되어 있으며 단어 사이에는 '_'으로 구분됨."
        + "반드시 대답은 '단어_단어_단어' 형식으로만 제공되어야 함. 이는 parsing되어 사용될 것임."
        + "예시: 'butterfly_나비_한국어'를 제공받을 시, '나비_나방_가방'으로 대답해야 함";

    [SerializeField] public bool isLoading = false;

    // AI_Manager 인스턴스화 싱글톤 패턴
    private static AI_Manager _instance;
    public static AI_Manager Instance
    {
        get {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(AI_Manager)) as AI_Manager;

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
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        api = new OpenAIAPI(API_KEY);
    }

    // 단어 설명을 요구
    public void Get_Info()
    {
        string word = WordManager.Instance.m_korean;
        string command = word + "이라는 단어에 대해 한 문장으로 설명해줘.";

        GetResponse(command);
    }

    // 추가 단어 설명을 요구
    public void Get_AddInfo()
    {
        string word = WordManager.Instance.m_korean;
        string command = word + "이라는 단어에 대해 두 문장으로 방금과 다르게 설명해줘.";

        GetResponse(command);
    }

    // 초기화 및 구동 설정 (해당 씬 대표 매니저에서 한번씩 호출)
    public void Init_AI(string st = "info")
    {
        string prompts;

        state = st;

        if (state == "info") // 설명 모드
        {
            prompts = prompts_info;
        }
        else // 퀴즈용
        {
            prompts = prompts_quiz;
        }

        messages = new List<ChatMessage> { new ChatMessage(ChatMessageRole.System, prompts) };
        message_record.Clear();
    }

    // 입력을 전송하고 대답을 받는 비동기 메서드
    private async void GetResponse(string text)
    {
        isLoading = true;

        // 입력한 메세지를 가져옴
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = text;

        //list에 메세지 추가
        messages.Add(userMessage);
        message_record.Add(userMessage.Content);

        // 전체 채팅을 openAI 서버에전송하여 다음 메시지(응답)를 가져오도록
        var chatResult = await api.Chat.CreateChatCompletionAsync(
            new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.1,
                MaxTokens = 200,
                Messages = messages,
            }
        );

        //응답 가져오기
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = ChatMessageRole.Assistant;
        responseMessage.Content = chatResult.Choices[0].Message.Content;

        //응답을 message리스트에 추가
        messages.Add(responseMessage);
        string msg = (responseMessage.Content).Replace(Environment.NewLine, ""); // 개행문자를 없앰
        message_record.Add(msg);

        if (state == "info")
        {
            string newInfo = msg.Replace(WordManager.Instance.m_korean, WordManager.Instance.m_english);
            try
            {
                newInfo = newInfo.Replace(WordManager.Instance.m_english + "은", WordManager.Instance.m_english + "은(는)");
            }
            catch
            {
                Debug.Log("해당 단어 없음");
            }
            DrawManager.Instance.info = newInfo;
            DrawManager.Instance.Set_AIText();
        }
        else
        {

        }

        isLoading = false;
    }
}
