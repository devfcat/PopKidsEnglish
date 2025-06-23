using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// QuizQuestion.cs
[System.Serializable]
public class QuizQuestion
{
    public string question;
    public List<string> answers;
    public int correctAnswerIndex;
    public string correctWord;
}
