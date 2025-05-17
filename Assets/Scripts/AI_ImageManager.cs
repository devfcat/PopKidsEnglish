using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Text; // 🔹 Encoding을 사용하려면 필요
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// JSON 구조 매핑용 클래스
[System.Serializable]
public class ImageRequest
{
    public string model;
    public string prompt;
    public int n;
    public string size;
}

public class AI_ImageManager : MonoBehaviour
{
    private string apiKey;
    public string m_word;
    private string prompt;
    public RawImage rawImage;
    private string filepath;
    public string imageUrl;

    private string target_word;

    void OnEnable()
    {
        target_word = WordManager.Instance.m_english;
        filepath = Application.persistentDataPath + "/Card/" + target_word + ".png";
        apiKey = AI_Manager.Instance.API_KEY;
        GetImage(target_word);
    }

    void GetImage(string word)
    {
        if (File.Exists(filepath))
        {
            Debug.Log(filepath + "에 파일이 있습니다.");
            StartCoroutine(LoadAIPNG());
        }
        else
        {
            m_word = word;
            string imagePrompt = "a storybook style drawing of " + word + " for children";
            StartCoroutine(GenerateImageFromPrompt(imagePrompt));
        }
    }

    private IEnumerator GenerateImageFromPrompt(string prompt)
    {
        AI_Manager.Instance.isLoading = true;

        string url = "https://api.openai.com/v1/images/generations";

        // JSON Body 구성
        string jsonBody = JsonUtility.ToJson(new ImageRequest
        {
            model = "dall-e-3", // 또는 "gpt-image-1" 사용 가능
            prompt = prompt,
            n = 1,
            size = "1024x1024"
        });

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("OpenAI 요청 실패: " + request.error);
            yield break;
        }

        string responseText = request.downloadHandler.text;
        try
        {
            JObject json = JObject.Parse(responseText);
            JArray dataArray = (JArray)json["data"];
            if (dataArray != null && dataArray.Count > 0)
            {
                url = dataArray[0]["url"]?.ToString();
                Debug.Log("이미지 URL: " + url);
                imageUrl = url;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON 파싱 오류: " + e.Message);
        }

        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("이미지 URL 파싱 실패");
            yield break;
        }

        // 이미지 다운로드
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = new Texture2D(2,2);   
            tex = DownloadHandlerTexture.GetContent(imageRequest);
            StartCoroutine(SaveAIPNG(tex));
        }
        else
        {
            Debug.LogError("이미지 다운로드 실패: " + imageRequest.error);
        }
    }

    // 이미지 저장 코루틴 (기존 SaveAIPNG)
    public IEnumerator SaveAIPNG(Texture2D pic)
    {
        yield return new WaitForEndOfFrame();

        byte[] bytes = pic.EncodeToPNG();
        Destroy(pic);

        string folderPath = Path.GetDirectoryName(filepath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.WriteAllBytes(filepath, bytes);

        StartCoroutine(LoadAIPNG());
    }

    IEnumerator LoadAIPNG()
    {
        byte[] byteTexture = System.IO.File.ReadAllBytes(filepath);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(byteTexture);
        rawImage.texture = tex;

        AI_Manager.Instance.isLoading = false;

        yield return null;
    }

}
