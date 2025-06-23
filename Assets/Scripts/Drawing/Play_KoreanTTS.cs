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
using TMPro;

/// <summary>
/// 단어장 카드가 앞면이 될 때마다 한국어 설명을 재생해주는 스크립트
/// </summary>
public class Play_KoreanTTS : MonoBehaviour
{
    public TextMeshProUGUI m_comment_tmp; // 한국어 설명이 담긴 tmp

    [Header("TTS Url")]
    public string apiUrl;

    [Header("API_Key")]
    private string apiKey; // ChatGPT API Key

    public string lastComment = "";

    // 싱글톤 인스턴스
    private static Play_KoreanTTS _instance;
    public static Play_KoreanTTS Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(Play_KoreanTTS)) as Play_KoreanTTS;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    void Start()
    {
        apiKey = AI_Manager.Instance.API_KEY;

        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }

    public async void Play_Info(string m_text)
    {
        if (m_text == "") return;
        if (lastComment == m_text)
        {
            StartCoroutine(Load_MP3());
            return;
        }
        
        string comment = m_text;
        string filePath = Path.Combine(Application.persistentDataPath, "Say/korean_tts.mp3");

        if (lastComment != comment)
        {
            Create_Word(comment);
            lastComment = comment;
        }
    }

    public IEnumerator Load_MP3()
    {
        Debug.Log("음성 파일을 불러옵니다");

        string filePath = Path.Combine(Application.persistentDataPath, "Say/korean_tts.mp3");
        Debug.Log(filePath);

        if (!File.Exists(filePath))
        {
            Debug.LogError("불러오기 실패");
            GameManager.Instance.ErrorCode = 201;
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
                yield break;
            }

            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    public async Task Create_Word(string text_word)
    {
        Debug.Log("음성 파일을 생성합니다");

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestData = new
            {
                model = "tts-1",
                input = text_word + ".",
                voice = "echo",
                speed = 1.0
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

                string filePath = Path.Combine(Application.persistentDataPath, "Say/korean_tts.mp3");
                File.WriteAllBytes(filePath, audioData);

                StartCoroutine(Load_MP3());

                Debug.Log(filePath);
            }
            else
            {
                Debug.LogError("음성 파일 생성 실패: " + response.StatusCode);  
                GameManager.Instance.ErrorCode = 200;
            }
        }

        StartCoroutine(Load_MP3());
    }

    // AudioSource 정지 메서드
    public void Stop()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    // AudioSource 일시정지 메서드
    public void Pause()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    // AudioSource 재생 메서드
    public void Resume()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }

    // 현재 재생 중인지 확인
    public bool IsPlaying()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        return audioSource != null && audioSource.isPlaying;
    }
}
