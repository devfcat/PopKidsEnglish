using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiz_Main : MonoBehaviour
{
    /// <summary>
    /// 패널이 활성화될 때마다 호출되어 퀴즈를 초기화하고 새로 시작
    /// </summary>
    void OnEnable()
    {
        QuizMaker.Instance.ResetQuizState();
        QuizMaker.Instance.StartQuiz();
    }
}
