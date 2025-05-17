using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VectorGraphics;
using LS.DrawTexture.Runtime;
using System.Collections;
using System.IO;
using System;

public class Word_Draw : MonoBehaviour
{
    [Header("DrawTexture")]
    public DrawTextureUI drawing;

    [Header("선 그리기 설정")] 
    [SerializeField] private int currentBrush; // 0이 지우개, 1이 연필
    public List<Image> brushes;
    public List<Sprite> brushes_img;
    [SerializeField] private Color currentColor; // 선 색상
    [SerializeField][Range(0.1f, 0.9f)] private float currentSize; // 선 굵기

    [Header("그림 리소스")]
    [SerializeField] private GameObject[] colors; // 색 고르기 UI

    [Header("그림")]
    public Texture2D m_picture;

    public TextMeshProUGUI word;
    public TextMeshProUGUI guideLine;

    public void OnEnable()
    {
        word.text = WordManager.Instance.m_english;
        guideLine.text = WordManager.Instance.m_english;

        drawing.Clear();

        // 초기 설정
        currentColor = colors[0].transform.GetChild(0).GetComponent<SVGImage>().color;
        drawing.Color = currentColor;
        colors[0].transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
        drawing.Size = 0.9f;
        SelectBrush(1);
    }

    public void Onclick_Eraser()
    {
        SelectBrush(0);
    }
    public void Onclick_Pencil()
    {
        SelectBrush(1);
    }

    public void Onclick_done()
    {
        SoundManager.Instance.PlaySFX(SFX.pretty);

        m_picture = new Texture2D(2, 2);   
        m_picture = drawing.GetTexture2D();
        StartCoroutine(SavePNG(m_picture));
    }

    // 그린 그림을 파일로 저장
    public IEnumerator SavePNG(Texture2D pic)
    {
        yield return new WaitForEndOfFrame();

        Texture2D tex = pic;
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        string filepath = Application.persistentDataPath + "/" + WordManager.Instance.m_section + "_" + WordManager.Instance.id + ".png";
        File.WriteAllBytes(filepath, bytes);

        GoToResult();
    }

        public void GoToResult()
    {
        GameManager.Instance.SetState(eState.Word_DrawResult);
    }


    // 1이 연필
    public void SelectBrush(int num)
    {
        currentBrush = num;

        // 연필을 골랐을 때
        if (num == 1)
        {
            drawing.IdTexture = 1;
            drawing.Type = DrawTextureUI.BrushType.none;
            brushes[1].sprite = brushes_img[2];
            brushes[0].sprite = brushes_img[1];
        }
        else // 지우개
        {
            drawing.IdTexture = 4;
            drawing.Type = DrawTextureUI.BrushType.eraser;
            brushes[0].sprite = brushes_img[0];
            brushes[1].sprite = brushes_img[3];
        }
    }

    public void SelectSize(Slider s) // 선 굵기 조절 Slider UI (On Value Changed)
    {
        currentSize = s.value; // 선 굵기는 정수 단위로 조절
        drawing.Size = currentSize;
    }

    public void SelectColor(SVGImage svg) // 선 색상 결정
    {
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f); // UI 크기 정상화
        }

        currentColor = svg.color;
        drawing.Color = currentColor;

        svg.transform.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f); // 선택된 색 UI는 크기가 줄어듦
    }

}

