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
        // 네이티브 갤러리를 활용해 스크린샷을 외부로 공유하며 퀴즈 결과를 텍스트로 설명에 첨부함
        StartCoroutine(CaptureAndShare());
    }
    
    /// <summary>
    /// 스크린샷을 찍고 공유하는 코루틴
    /// </summary>
    private IEnumerator CaptureAndShare()
    {
        // 다음 프레임까지 대기
        yield return new WaitForEndOfFrame();
        
        // 스크린샷 찍기
        Texture2D screenshot = CaptureScreenshot();
        
        // 공유할 텍스트 생성
        string shareText = CreateShareText();
        
        // 네이티브 공유 기능 사용
        ShareScreenshot(screenshot, shareText);
        
        // 메모리 정리
        Destroy(screenshot);
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
    
    /// <summary>
    /// 스크린샷을 외부로 공유
    /// </summary>
    private void ShareScreenshot(Texture2D screenshot, string shareText)
    {
        // 스크린샷을 바이트 배열로 변환
        byte[] screenshotBytes = screenshot.EncodeToPNG();
        
        // 임시 파일 경로 생성
        string filePath = Path.Combine(Application.temporaryCachePath, "quiz_result.png");
        
        // 파일로 저장
        File.WriteAllBytes(filePath, screenshotBytes);
        
        // 네이티브 공유 기능 호출
        #if UNITY_ANDROID
            ShareAndroid(filePath, shareText);
        #elif UNITY_IOS
            ShareIOS(filePath, shareText);
        #endif
    }
    
    #if UNITY_ANDROID
    /// <summary>
    /// Android에서 공유 기능 실행
    /// </summary>
    private void ShareAndroid(string filePath, string shareText)
    {
        using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
        using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent"))
        {
            intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intent.Call<AndroidJavaObject>("setType", "image/png");
            
            // 파일 URI 생성
            using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaClass fileProviderClass = new AndroidJavaClass("androidx.core.content.FileProvider"))
            using (AndroidJavaObject file = new AndroidJavaObject("java.io.File", filePath))
            {
                AndroidJavaObject uri = fileProviderClass.CallStatic<AndroidJavaObject>("getUriForFile", 
                    GetCurrentActivity(), 
                    Application.identifier + ".fileprovider", 
                    file);
                
                intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uri);
                intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);
                intent.Call<AndroidJavaObject>("addFlags", intentClass.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION"));
                
                // 공유 다이얼로그 표시
                AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, "퀴즈 결과 공유하기");
                GetCurrentActivity().Call("startActivity", chooser);
            }
        }
    }
    
    /// <summary>
    /// 현재 Activity 가져오기
    /// </summary>
    private AndroidJavaObject GetCurrentActivity()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
    #endif
    
    #if UNITY_IOS
    /// <summary>
    /// iOS에서 공유 기능 실행
    /// </summary>
    private void ShareIOS(string filePath, string shareText)
    {
        // iOS에서는 NativeGallery 플러그인을 사용하거나
        // 직접 네이티브 코드를 호출해야 합니다.
        // 여기서는 간단한 예시만 제공합니다.
        Debug.Log("iOS 공유 기능: " + filePath + "\n" + shareText);
    }
    #endif
}
