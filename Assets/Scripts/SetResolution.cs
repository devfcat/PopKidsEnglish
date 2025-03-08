using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum screen_mod
{
    portrait = 0,
    landscape = 1
}

/// <summary>
/// 기기 해상도에 따라 화면 비율 맞춤
/// </summary>
public class SetResolution : MonoBehaviour
{
    public screen_mod m_mod;
    public List<GameObject> panels;
    [SerializeField] private float screenWidth;
    [SerializeField] private float screenHeight;

    private float m_width;
    private float m_height;

    void Start()
    {
        Get_ScreenSize();
        Set_Panel();
    }

    void Get_ScreenSize()
    {
        screenWidth = GameManager.Instance.screen_width;
        screenHeight = GameManager.Instance.screen_height;
    }

    void Set_Panel()
    {
        if (m_mod == screen_mod.portrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            if (screenWidth/screenHeight >= 0.5625f) // 9:16보다 정사각형에 가까움
            {
                // 높이를 기준으로 함
                m_width = screenHeight * 0.5625f;
                m_height = screenHeight;
            }
            else // 9:16보다 길쭉함
            {
                // 너비를 기준으로 함
                m_width = screenWidth;
                m_height = screenWidth * 1.777f;
            }
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeRight;
            if (screenWidth/screenHeight >= 0.5625f) // 9:16보다 정사각형에 가까움
            {
                // 높이를 기준으로 함
                m_width = screenHeight;
                m_height = screenHeight * 0.5625f;
            }
            else // 9:16보다 길쭉함
            {
                // 너비를 기준으로 함
                m_width = screenWidth * 1.777f;
                m_height = screenWidth;
            }
        }

        for (int i = 0 ; i < panels.Count; i++)
        {
            panels[i].GetComponent<RectTransform>().sizeDelta = new Vector2(m_width, m_height);
        }
    }
}
