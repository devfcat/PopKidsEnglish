using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

/// <summary>
/// 퀴즈 점수와 퀴즈 유형 구성을 알려주는 패널
/// 퀴즈 결과를 NativeGallery를 사용하여 외부로 공유 가능하도록 함
/// </summary>
public class Quiz_Result : MonoBehaviour
{
    public TextMeshProUGUI score_UI;
    public TextMeshProUGUI type_UI_1; // 퀴즈 유형 1
    public TextMeshProUGUI type_UI_2; // 퀴즈 유형 2

    void OnEnable()
    {
        score_UI.text = $"{QuizManager.Instance.score} / 5 ({QuizManager.Instance.score*20}점)";
        type_UI_1.text = $"한글 뜻 맞추기 퀴즈 {QuizMaker.Instance.quiz_type_1}문제";
        type_UI_2.text = $"영단어 맞추기 퀴즈 {QuizMaker.Instance.quiz_type_2}문제";
    }

    public void OnClick_Exit()
    {
        GameManager.Instance.SetState(eState.Main_WordBook);
    }

    public void OnClick_Share()
    {
        SoundManager.Instance.PlaySFX(SFX.pretty);
        StartCoroutine(CaptureAndShare());
    }
    
    /// <summary>
    /// 스크린샷을 찍고 공유하는 코루틴
    /// </summary>
    private IEnumerator CaptureAndShare()
    {
        yield return new WaitForEndOfFrame();
        
        // 스크린샷 찍기
        Texture2D screenshot = CaptureScreenshot();
        
        // 임시 파일로 저장
        string filePath = Path.Combine(Application.temporaryCachePath, "quiz_result.png");
        File.WriteAllBytes(filePath, screenshot.EncodeToPNG());
        Destroy(screenshot);
        
        // 공유할 텍스트 생성
        string shareText = CreateShareText();
        
        // NativeShare 사용
        new NativeShare()
            .AddFile(filePath)
            .SetSubject("PopKids English 퀴즈 결과")
            .SetText(shareText)
            .SetTitle("퀴즈 결과 공유")
            .Share();
    }
    
    /// <summary>
    /// 현재 화면의 스크린샷을 캡처
    /// </summary>
    private Texture2D CaptureScreenshot()
    {
        // 현재 화면 해상도 가져오기
        int width = Screen.width;
        int height = Screen.height;
        
        // 스크린샷 텍스처 생성
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        // 스크린샷 찍기
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();
        
        return screenshot;
    }
    
    /// <summary>
    /// 공유할 텍스트 생성
    /// </summary>
    private string CreateShareText()
    {
        int score = QuizManager.Instance.score;
        int totalQuestions = 5;
        int percentage = (int)((float)score / totalQuestions * 100);
        
        string shareText = $"🎉 PopKids English 퀴즈 결과 🎉\n\n";
        shareText += $"점수: {score}/{totalQuestions} ({percentage}점)\n\n";
        
        // 점수에 따른 메시지 추가
        if (percentage >= 80)
        {
            shareText += "🌟 훌륭해요! 영어 실력이 정말 좋네요! 🌟";
        }
        else if (percentage >= 60)
        {
            shareText += "👍 잘했어요! 조금만 더 노력하면 완벽해요! 👍";
        }
        else
        {
            shareText += "💪 괜찮아요! 다음에는 더 잘할 수 있을 거예요! 💪";
        }
        
        return shareText;
    }
}
