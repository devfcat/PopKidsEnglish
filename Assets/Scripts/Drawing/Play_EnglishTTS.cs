using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Android;
using NLayer;

public class Play_EnglishTTS : MonoBehaviour
{
    [Header("TTS Url")]
    public string apiUrl;

    [Header("API_Key")]
    [SerializeField] private string apiKey; // ChatGPT API Key

    public string this_word; // 현재 음성파일의 단어

    void Start()
    {
        apiKey = AI_Manager.Instance.API_KEY;

        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            // 카메라 권한이 없다면 권한을 요청함
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            // 카메라 권한이 없다면 권한을 요청함
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }

    /// <summary>
    /// 이전에 조회했던 단어거나 이미 파일에 있는 단어면 파일 불러오기
    /// 없을 때만 api롤 생성하여 가져온다.
    /// </summary>
    public async void OnClick_Listen()
    {
        this_word = WordManager.Instance.m_english;

        string filePath = Path.Combine(Application.persistentDataPath, "Say/" + this_word + ".mp3");

        if (File.Exists(filePath))
        {
            StartCoroutine(Load_MP3());
        }
        else
        {
            await Create_Word(this_word);
        }
    }

    public async Task Create_Word(string text_word)
    {
        Debug.Log("음성 파일을 생성합니다");
        AI_Manager.Instance.isLoading = true;

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestData = new
            {
                model = "tts-1",
                input = text_word + ".",
                voice = "alloy"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();
                string base64Audio = Convert.ToBase64String(audioBytes);

                byte[] audioData = Convert.FromBase64String(base64Audio);

                if(!Directory.Exists(Application.persistentDataPath + "/Say"))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/Say");
                }

                string filePath = Path.Combine(Application.persistentDataPath, "Say/" + this_word + ".mp3");
                File.WriteAllBytes(filePath, audioData);

                if (!File.Exists(filePath))
                {
                    GameManager.Instance.ErrorCode = 200;
                    AI_Manager.Instance.isLoading = false;
                }
                else
                {
                    StartCoroutine(Load_MP3());
                }

                Debug.Log(filePath);
            }
            else
            {
                GameManager.Instance.ErrorCode = 200;
                AI_Manager.Instance.isLoading = false;
            }
        }
    }

    public IEnumerator Load_MP3()
    {
        Debug.Log("음성 파일을 불러옵니다");

        string filePath = Path.Combine(Application.persistentDataPath, "Say/" + this_word + ".mp3");
        Debug.Log(filePath);

        if (!File.Exists(filePath))
        {
            Debug.LogError("불러오기 실패");
            GameManager.Instance.ErrorCode = 201;
            AI_Manager.Instance.isLoading = false;
        }

        string fileUrl = "file://" + filePath;
        Debug.Log(fileUrl);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileUrl, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("불러오기 실패: " + www.error);
                GameManager.Instance.ErrorCode = 201;
                AI_Manager.Instance.isLoading = false;
                yield break;
            }

            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        /*
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);

        using (var mp3Stream = new MemoryStream(fileData))
        using (var mpegFile = new MpegFile(mp3Stream))
        {
            float[] samples = new float[mpegFile.Length / sizeof(float)];
            int sampleCount = mpegFile.ReadSamples(samples, 0, samples.Length);

            AudioClip audioClip = AudioClip.Create("MP3Clip", sampleCount, mpegFile.Channels, 22050, false);
            audioClip.SetData(samples, 0);

            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        */

        AI_Manager.Instance.isLoading = false;

        yield return null;
    }
}
