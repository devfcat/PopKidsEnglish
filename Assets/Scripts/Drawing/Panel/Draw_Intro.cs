using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Intro : MonoBehaviour
{
    public void Onclick()
    {
        GameManager.Instance.SetState(eState.Draw);
    }
}
