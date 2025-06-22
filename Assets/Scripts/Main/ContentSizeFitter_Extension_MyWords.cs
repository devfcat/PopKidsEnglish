using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 내 단어장 목록 조회 시 목록에 맞게 content 크기를 조절해주는 스크립트
/// </summary>
public class ContentSizeFitter_Extension_MyWords : MonoBehaviour
{
    public float min_size = 1600;
    public int value_words;

    public void Check_State()
    {
        value_words = LearnSelect_View.Instance.instanceBoxes.Count;
    }

    public void Size_Fitter()
    {
        Check_State();

        Vector2 m_size = this.transform.GetComponent<RectTransform>().sizeDelta;
        float calculatedHeight = min_size + 500f + 275f * (value_words - 3);
        float finalHeight = Mathf.Max(calculatedHeight, min_size); // min_size보다 작지 않도록 보장
        
        this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(m_size.x, finalHeight);
    
        Debug.Log("value_words: " + value_words);
    }
}
