using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// ChatGPT API를 사용하여 퀴즈 문제와 선택지 5개를 생성해 보여주는 패널 클래스
/// </summary>
public class QuizMaker : MonoBehaviour
{
    [Header("퀴즈 UI 요소")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<TextMeshProUGUI> answerTexts;
    [SerializeField] private List<GameObject> answerButtons;
    [SerializeField] private TextMeshProUGUI index_UI;

    [Header("퀴즈 데이터")]
    [SerializeField] private List<QuizQuestion> quizQuestions;
    [SerializeField] private int currentQuestionIndex = 0;
    [SerializeField] private int correctAnswers = 0;
    public int quiz_type_1; // 한글 뜻 맞추기 유형 퀴즈 개수
    public int quiz_type_2; // 영단어 맞추기 유형 퀴즈 개수
    
    [Header("로딩 처리")]
    [SerializeField] private GameObject loadingPanel;
    [Header("정답 또는 오답 효과 팝업")]
    [SerializeField] private GameObject popup_correct;
    [SerializeField] private GameObject popup_wrong;
    [Header("퀴즈 완료 팝업")]
    [SerializeField] private GameObject popup_done;
    
    // 퀴즈 문제 데이터 구조
    [System.Serializable]
    public class QuizQuestion
    {
        public string question;
        public List<string> answers;
        public int correctAnswerIndex;
        public string correctWord;
    }
    
    // QuizMaker 인스턴스화 싱글톤 패턴
    private static QuizMaker _instance;
    public static QuizMaker Instance
    {
        get {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(QuizMaker)) as QuizMaker;

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
    }
    
    void Start()
    {
        quizQuestions = new List<QuizQuestion>();
    }
    
    /// <summary>
    /// 퀴즈 생성 시작
    /// </summary>
    public void StartQuiz()
    {
        // 기존 퀴즈 상태 정리
        ResetQuizState();
        StartCoroutine(GenerateQuiz());
    }
    
    /// <summary>
    /// 퀴즈 상태 초기화
    /// </summary>
    public void ResetQuizState()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;
        quiz_type_1 = 0;
        quiz_type_2 = 0;
        
        // 퀴즈 문제 리스트 초기화
        if (quizQuestions != null)
        {
            quizQuestions.Clear();
        }
        else
        {
            quizQuestions = new List<QuizQuestion>();
        }
        
        // 팝업들 비활성화
        if (popup_correct != null) popup_correct.SetActive(false);
        if (popup_wrong != null) popup_wrong.SetActive(false);
        if (popup_done != null) popup_done.SetActive(false);
        
        // 기존 코루틴 중지
        StopAllCoroutines();
    }
    
    /// <summary>
    /// WordManager의 Section에 해당하는 단어장에서 퀴즈 생성
    /// </summary>
    private IEnumerator GenerateQuiz()
    {
        loadingPanel.SetActive(true);
        
        // 퀴즈 유형 카운터 초기화
        quiz_type_1 = 0;
        quiz_type_2 = 0;
        
        // WordManager에서 현재 섹션의 단어 리스트 가져오기
        List<Word> availableWords = WordManager.Instance.wordList;
        
        if (availableWords == null || availableWords.Count == 0)
        {
            Debug.LogError("단어 리스트가 비어있습니다!");
            loadingPanel.SetActive(false);
            yield break;
        }
        
        // 단어가 5개 미만이면 모든 단어 사용, 5개 이상이면 랜덤으로 5개 선택
        List<Word> selectedWords = new List<Word>();
        if (availableWords.Count <= 5)
        {
            selectedWords = availableWords.ToList();
        }
        else
        {
            // 랜덤으로 5개 단어 선택
            List<Word> shuffledWords = availableWords.OrderBy(x => Random.Range(0f, 1f)).ToList();
            selectedWords = shuffledWords.Take(5).ToList();
        }
        
        // 각 단어에 대해 퀴즈 문제 생성
        quizQuestions.Clear();
        
        foreach (Word word in selectedWords)
        {
            QuizQuestion quizQuestion = CreateQuizQuestion(word, availableWords);
            quizQuestions.Add(quizQuestion);
        }
        
        // 퀴즈 문제 순서 섞기
        quizQuestions = quizQuestions.OrderBy(x => Random.Range(0f, 1f)).ToList();
        
        loadingPanel.SetActive(false);
        
        // 첫 번째 문제 표시
        currentQuestionIndex = 0;
        correctAnswers = 0;
        DisplayCurrentQuestion();
    }
    
    /// <summary>
    /// 개별 퀴즈 문제 생성
    /// </summary>
    private QuizQuestion CreateQuizQuestion(Word targetWord, List<Word> allWords)
    {
        QuizQuestion question = new QuizQuestion();
        
        // 문제 유형 랜덤 선택 (1: 영단어 뜻 맞추기, 2: 한글 뜻에 맞는 영단어)
        int questionType = Random.Range(1, 3);
        
        if (questionType == 1)
        {
            // 영단어 뜻 맞추기
            question.question = $"{targetWord.english}의 뜻은 무엇일까요?";
            question.correctWord = targetWord.korean;
            quiz_type_1++; // 한글 뜻 맞추기 유형 카운트
        }
        else
        {
            // 한글 뜻에 맞는 영단어
            question.question = $"{targetWord.korean}은(는) 영어로 무엇일까요?";
            question.correctWord = targetWord.english;
            quiz_type_2++; // 영단어 맞추기 유형 카운트
        }
        
        // 정답과 오답 3개 선택지 생성
        question.answers = new List<string>();
        
        // 정답 추가
        if (questionType == 1)
        {
            question.answers.Add(targetWord.korean);
        }
        else
        {
            question.answers.Add(targetWord.english);
        }
        
        // 오답 3개 생성 (다른 단어들에서 랜덤 선택)
        List<Word> otherWords = allWords.Where(w => w.english != targetWord.english).ToList();
        
        if (otherWords.Count >= 3)
        {
            // 다른 단어들 중에서 랜덤으로 3개 선택
            List<Word> randomWords = otherWords.OrderBy(x => Random.Range(0f, 1f)).Take(3).ToList();
            
            foreach (Word randomWord in randomWords)
            {
                if (questionType == 1)
                {
                    question.answers.Add(randomWord.korean);
                }
                else
                {
                    question.answers.Add(randomWord.english);
                }
            }
        }
        else
        {
            // 단어가 부족한 경우 기본 오답 추가
            string[] defaultWrongAnswers = questionType == 1 
                ? new string[] { "잘못된 답", "틀린 답", "오답" }
                : new string[] { "wrong", "incorrect", "false" };
                
            for (int i = 0; i < 3; i++)
            {
                question.answers.Add(defaultWrongAnswers[i]);
            }
        }
        
        // 선택지 순서 섞기
        question.answers = question.answers.OrderBy(x => Random.Range(0f, 1f)).ToList();
        
        // 정답 인덱스 찾기
        if (questionType == 1)
        {
            question.correctAnswerIndex = question.answers.IndexOf(targetWord.korean);
        }
        else
        {
            question.correctAnswerIndex = question.answers.IndexOf(targetWord.english);
        }
        
        return question;
    }
    
    /// <summary>
    /// 현재 문제 표시
    /// </summary>
    private void DisplayCurrentQuestion()
    {
        // 안전성 검사 추가
        if (quizQuestions == null || quizQuestions.Count == 0)
        {
            Debug.LogWarning("퀴즈 문제가 없습니다.");
            return;
        }
        
        // currentQuestionIndex가 범위를 벗어났는지 먼저 체크
        if (currentQuestionIndex >= quizQuestions.Count)
        {
            // 퀴즈 완료
            ShowQuizResult();
            return;
        }
        
        // currentQuestionIndex가 음수인 경우 처리
        if (currentQuestionIndex < 0)
        {
            Debug.LogWarning("현재 문제 인덱스가 음수입니다. 0으로 초기화합니다.");
            currentQuestionIndex = 0;
        }
        
        QuizQuestion currentQuestion = quizQuestions[currentQuestionIndex];
        
        // UI 요소 null 체크
        if (questionText == null)
        {
            Debug.LogError("questionText가 null입니다.");
            return;
        }
        
        // 문제 텍스트 설정
        questionText.text = $"{currentQuestion.question}";
        
        // 선택지 설정
        for (int i = 0; i < answerTexts.Count; i++)
        {
            if (i < currentQuestion.answers.Count)
            {
                if (answerTexts[i] != null)
                {
                    answerTexts[i].text = currentQuestion.answers[i];
                }
                if (answerButtons[i] != null)
                {
                    answerButtons[i].SetActive(true);
                }
            }
            else
            {
                if (answerButtons[i] != null)
                {
                    answerButtons[i].SetActive(false);
                }
            }
        }

        if (index_UI != null)
        {
            index_UI.text = $"{currentQuestionIndex + 1}";
        }
    }
    
    /// <summary>
    /// 답변 선택 처리
    /// </summary>
    public void OnAnswerSelected(int answerIndex)
    { 
        QuizQuestion currentQuestion = quizQuestions[currentQuestionIndex];
        
        // answerIndex 범위 검사
        if (answerIndex < 0 || answerIndex >= currentQuestion.answers.Count)
        {
            Debug.LogWarning($"답변 인덱스 {answerIndex}가 범위를 벗어났습니다. (0-{currentQuestion.answers.Count - 1})");
            return;
        }
        
        // 정답 체크
        if (answerIndex == currentQuestion.correctAnswerIndex)
        {
            correctAnswers++;
            popup_correct.SetActive(true);
            SoundManager.Instance.PlaySFX(SFX.correct);
        }
        else
        {
            popup_wrong.SetActive(true);
            SoundManager.Instance.PlaySFX(SFX.uncorrect);
        }
        
        // 다음 문제로 이동
        currentQuestionIndex++;
        
        // 다음 문제가 있는지 확인 후 표시
        if (currentQuestionIndex >= quizQuestions.Count)
        {
            // 퀴즈 완료
            ShowQuizResult();
        }
        else
        {
            // 다음 문제 표시
            DisplayCurrentQuestion();
        }
    }
    
    /// <summary>
    /// 퀴즈 결과 표시
    /// </summary>
    private void ShowQuizResult()
    {
        // popup_done.SetActive(true);
        SoundManager.Instance.PlaySFX(SFX.Done);
        
        QuizManager.Instance.score = correctAnswers;    
        // QuizManager의 결과 상태로 전환
        GameManager.Instance.SetState(eState.Quiz_Result);
        
        // 3초 뒤에 팝업 자동으로 끄기
        StartCoroutine(HideDonePopupAfterDelay());
    }
    
    /// <summary>
    /// 3초 뒤에 완료 팝업을 끄는 코루틴
    /// </summary>
    private IEnumerator HideDonePopupAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        popup_done.SetActive(false);
    }
}
