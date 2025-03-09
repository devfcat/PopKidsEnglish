using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    
    public void Booting()
    {
        GameManager.Instance.SetState(eState.Main_Menu);
    }
}
