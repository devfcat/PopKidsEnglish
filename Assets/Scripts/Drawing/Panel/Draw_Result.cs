using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Draw_Result : MonoBehaviour
{
    [Header("그림을 보여주는 액자")]
    public GameObject screen_paint; // 그림을 보여줄 공간

    void OnEnable()
    {
        SoundManager.Instance.PlaySFX(SFX.Done);
        StartCoroutine(GetMyPicture());
    }

    // AR로 보기
    public void View_AR()
    {

    }

    public void Onclick_home()
    {
        GameManager.Instance.SetState(eState.Main_Menu);
    }

    // 이전 화면에서 내가 그린 그림을 가져와 적용함
    IEnumerator GetMyPicture()
    {
        string filepath = Application.persistentDataPath + "/myPainting" + ".png";
        
        byte[] byteTexture = System.IO.File.ReadAllBytes(filepath);
        //Debug.Log(byteTexture.Length);

        Texture2D tex = new Texture2D(2, 2);   
        tex.LoadImage(byteTexture);

        screen_paint.GetComponent<RawImage>().texture = tex;

        // Destroy(tex);

        yield return null;
    }

    // 내가 그림을 갤러리에 저장함
    public void SaveMyPicture()
    {
        Texture2D tex = new Texture2D(2,2);
        tex = screen_paint.GetComponent<RawImage>().texture as Texture2D;
        StartCoroutine(CaptureScreenshotAndSave(tex));
    }

    private IEnumerator CaptureScreenshotAndSave(Texture2D tex)
    {
        yield return new WaitForEndOfFrame();
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write,NativeGallery.MediaType.Image);

        if (permission == NativeGallery.Permission.Denied)
        {
            if (NativeGallery.CanOpenSettings())
            {
                NativeGallery.OpenSettings();
            }
        }

        string photoName = DateTime.Now.ToString(("yyyy_MM_dd_HH_mm_ss"));
        string folderName = "POP_Kids_English";

        NativeGallery.SaveImageToGallery(tex, photoName, folderName);

        // 그림 저장 완료 팝업 띄움
        DrawManager.Instance.popup_saveDone.SetActive(true);
    }
}
