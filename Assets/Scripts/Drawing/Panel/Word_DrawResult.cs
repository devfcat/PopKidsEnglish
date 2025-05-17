using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Word_DrawResult : MonoBehaviour
{
    public GameObject btn_next; // 다음으로 버튼
    public TextMeshProUGUI word; // 정답을 보여주는 단어 박스

    [Header("그림을 보여주는 액자")]
    public RawImage screen_paint; // 그림을 보여줄 공간
    public Texture2D m_texture;

    void OnEnable()
    {
        SoundManager.Instance.PlaySFX(SFX.Done);

        word.text = WordManager.Instance.m_english + " / " + WordManager.Instance.m_korean;
        Set_NextButton();
        StartCoroutine(GetMyPicture());
    }

    // AR로 보기
    public void View_AR()
    {

    }

    // 단어장에서 이 단어가 마지막이 아니어야 다음으로 버튼이 나옴
    public void Set_NextButton()
    {
        int m_id = WordManager.Instance.id;
        int word_length = WordManager.Instance.wordList.Count;

        if (m_id - 1 >= word_length)
        {
            btn_next.SetActive(false);
        }
        else
        {
            btn_next.SetActive(true);
        }
    }

    // 다음 단어 학습 가능
    public void Onclick_Next()
    {
        int m_id = WordManager.Instance.id;
        WordManager.Instance.id = m_id + 1;
        m_id = WordManager.Instance.id;

        WordManager.Instance.m_english = WordManager.Instance.wordList[m_id].english;
        WordManager.Instance.m_korean = WordManager.Instance.wordList[m_id].korean;

        GameManager.Instance.SetState(eState.Word_Main);
    }

    // 이전 화면에서 내가 그린 그림을 가져와 적용함
    IEnumerator GetMyPicture()
    {
        string filepath = Application.persistentDataPath + "/" + WordManager.Instance.m_section + "_" + WordManager.Instance.id + ".png";

        byte[] byteTexture = System.IO.File.ReadAllBytes(filepath);
        if (byteTexture.Length > 0)
        {
            Texture2D tex = new Texture2D(2, 2);

            tex.LoadImage(byteTexture);
            // tex.Apply();

            m_texture = new Texture2D(2, 2);
            m_texture = tex;

            //Destroy(tex);
        }
        screen_paint.texture = m_texture;

        yield return null;
    }

    // 내가 그림을 갤러리에 저장함
    public void SaveMyPicture()
    {
        StartCoroutine(CaptureScreenshotAndSave());
    }

    private IEnumerator CaptureScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);

        if (permission == NativeGallery.Permission.Denied)
        {
            if (NativeGallery.CanOpenSettings())
            {
                NativeGallery.OpenSettings();
            }
        }

        string photoName = WordManager.Instance.m_section + "_" + WordManager.Instance.m_english;
        string folderName = "POP_Kids_English";

        NativeGallery.SaveImageToGallery(m_texture, folderName, photoName);

        // 그림 저장 완료 팝업 띄움
        DrawManager.Instance.popup_saveDone.SetActive(true);
    }

    public void OnClick_RePaint()
    {
        GameManager.Instance.SetState(eState.Word_Draw);
    }
}
