using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Navigation : MonoBehaviour
{
    [SerializeField] private int m_state;
    public List<Image> navigationIMGs;
    public List<Sprite> defaults;
    public List<Sprite> clicked;

    void Update()
    {
        if (GameManager.Instance.m_state == eState.Main_Menu)
        {
            Draw_Navigation(0);
        }
        else if (GameManager.Instance.m_state == eState.MyDrawing_Menu)
        {
            Draw_Navigation(2);
        }
        else
        {
            for (int i=0; i < navigationIMGs.Count; i++)
            {   
                navigationIMGs[i].sprite = defaults[i];
            }
        }
    }

    private void Draw_Navigation(int num)
    {
        for (int i=0; i < navigationIMGs.Count; i++)
        {
            if (i == num)
            {
                navigationIMGs[i].sprite = clicked[i];
            }
            else
            {
                navigationIMGs[i].sprite = defaults[i];
            }
        }
    }

    public void Click_Navigation(int num)
    {
        Draw_Navigation(num);

        switch(num)
        {
            case 0:
                GameManager.Instance.SetState(eState.Main_Menu);
                break;
            case 1:
                GameManager.Instance.SetState(eState.Draw_Intro);
                break;
            case 2:
                GameManager.Instance.SetState(eState.MyDrawing_Menu);
                break;
            default:
                break;
        }
    }

}
