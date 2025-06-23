using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AR 씬의 UI 기능 관리 스크립트
/// </summary>
public class ARManager : MonoBehaviour
{
    [SerializeField] private int isAudioActive; // 소리 켜짐(0) 꺼짐(1)
    public List<GameObject> btn_audios; // 오디오버튼

    void Start()
    {
        Get_Audio();
    }

    // 뒤로가기
    public void Onclick_Back()
    {
        // 이전 화면을 저장해서 뒤로가기 누를 시 이전 씬으로 이동함
        eState state = GameManager.Instance.prev_state;
        GameManager.Instance.SetState(state);
    }

    // 소리 버튼 초기 설정 (시작 시 한번만 실행됨)
    public void Get_Audio()
    {
        isAudioActive = PlayerPrefs.GetInt("isAudioActive");
        if (isAudioActive == 0) // 켜짐
        {
            SoundManager.Instance.Set_Volume(true);
            btn_audios[0].SetActive(true);
            btn_audios[1].SetActive(false);
        } 
        else // 꺼짐
        {
            SoundManager.Instance.Set_Volume(false);
            btn_audios[0].SetActive(false);
            btn_audios[1].SetActive(true);
        }
    }

    // 실제 적용되는 메서드
    public void Set_Audio()
    {
        if (isAudioActive == 0) // 켜짐
        {
            isAudioActive = 1; // 꺼지게 만듦
            SoundManager.Instance.Set_Volume(false);
            btn_audios[0].SetActive(false);
            btn_audios[1].SetActive(true);
        } 
        else // 꺼짐
        {
            isAudioActive = 0; // 켜지게 만듦
            SoundManager.Instance.Set_Volume(true);
            btn_audios[0].SetActive(true);
            btn_audios[1].SetActive(false);
        }
        PlayerPrefs.SetInt("isAudioActive", isAudioActive);
    }
}
