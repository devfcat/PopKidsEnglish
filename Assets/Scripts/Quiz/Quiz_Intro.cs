using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiz_Intro : MonoBehaviour
{
    public void OnClick_Next()
    {
        GameManager.Instance.SetState(eState.Quiz_Main);
    }
}
