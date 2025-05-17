using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentSizeExtension : MonoBehaviour
{
    public float min_size = 1600;
    public int value_words;

    public void Check_State()
    {
        value_words = WordManager.Instance.wordList.Count;
    }

    public void Size_Fitter()
    {
        Check_State();

        Vector2 m_size = this.transform.GetComponent<RectTransform>().sizeDelta;
        this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(m_size.x, min_size + 300f + 275f*(value_words-3));
    }
}
