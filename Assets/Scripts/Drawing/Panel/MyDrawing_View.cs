using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.IO;

public class MyDrawing_View : MonoBehaviour
{
    [Tooltip("현재 보고 있는 단어")] public List<TextMeshProUGUI> m_words;

    public bool isFront; // 카드가 앞면인가
    public List<GameObject> Cards;
    public RawImage card_image; // 카드 이미지
    public Texture2D m_texture;

    public List<GameObject> ui_list;

    async Task Init()
    {
        m_words[0].text = WordManager.Instance.m_english;
        m_words[1].text = WordManager.Instance.m_english + " / " + WordManager.Instance.m_korean;

        isFront = true;

        card_image.texture = null;
    }

    void OnEnable()
    {
        Init();
        Set_Image();
        Set_Cards();
    }

    // 내 단어장 카드의 이미지를 가져와 적용
    public void Set_Image()
    {
        string directoryPath = Application.persistentDataPath + "/MyData";
        string filepath = directoryPath + "/" + WordManager.Instance.m_section + "_" + WordManager.Instance.id + ".png";
        
        // MyData 디렉토리가 없으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("MyData 디렉토리를 생성했습니다: " + directoryPath);
        }
        
        if (File.Exists(filepath))
        {
            byte[] imageData = File.ReadAllBytes(filepath);
            m_texture = new Texture2D(2, 2);
            m_texture.LoadImage(imageData);
            card_image.texture = m_texture;
        }
        else
        {
            Debug.LogWarning("이미지 파일이 존재하지 않습니다: " + filepath);
            m_texture = null;
        }
    }

    public void Set_Cards()
    {
        if (isFront)
        {
            Play_EnglishTTS.Instance.OnClick_Listen();
            Cards[0].SetActive(true);
            Cards[1].SetActive(false);
        }
        else
        {
            Play_KoreanTTS.Instance.Play_Info(WordManager.Instance.m_korean);
            Cards[0].SetActive(false);
            Cards[1].SetActive(true);
        }
    }

    public void Flip_Card()
    {
        isFront = !isFront;
        Set_Cards();
    }

    // 현재 단어장 카드를 UI를 제외하고 캡쳐하여 휴대폰 갤러리에 저장할 수 있게 처리
    public void OnClick_SaveImage()
    {
        if (m_texture == null)
        {
            Debug.LogError("저장할 이미지가 없습니다!");
            return;
        }
        StartCoroutine(CaptureScreenshotAndSave());
    }

    public void Off_UI()
    {
        foreach (GameObject ui in ui_list)
        {
            ui.SetActive(false);
        }
    }

    public void On_UI()
    {
        foreach (GameObject ui in ui_list)
        {
            ui.SetActive(true);
        }
    }

    private IEnumerator CaptureScreenshotAndSave()
    {
        // m_texture가 null인지 다시 한번 확인
        if (m_texture == null)
        {
            Debug.LogError("저장할 이미지가 null입니다!");
            yield break;
        }

        Off_UI();

        yield return new WaitForEndOfFrame();
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);

        if (permission == NativeGallery.Permission.Denied)
        {
            if (NativeGallery.CanOpenSettings())
            {
                NativeGallery.OpenSettings();
            }
            On_UI();
            yield break;
        }

        string photoName = WordManager.Instance.m_section + "_" + WordManager.Instance.m_english;
        string folderName = "POP_Kids_English";

        try
        {
            NativeGallery.SaveImageToGallery(m_texture, folderName, photoName);
            Debug.Log("이미지가 갤러리에 저장되었습니다: " + photoName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("이미지 저장 중 오류 발생: " + e.Message);
        }

        On_UI();

        // 그림 저장 완료 팝업 띄움
        DrawManager.Instance.popup_saveDone.SetActive(true);
    }

    public void OnClick_ReDraw()
    {
        GameManager.Instance.SetState(eState.Word_Main);
    }

    // AR로 보기
    public void View_AR()
    {
        GameManager.Instance.SetState(eState.AR);
    }
}
