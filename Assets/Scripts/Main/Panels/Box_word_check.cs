using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

/// <summary>
/// 이 단어가 이미 학습되어 있는지 (단어장 그림 파일로 확인) 체크하여 학습 여부 메달을 표시함
/// </summary>
public class Box_word_check : MonoBehaviour
{
    public bool isLearned = false;
    public GameObject medal;
    public TextMeshProUGUI tmp; // 단어가 적혀있는 TextMesh 컴포넌트

    void OnEnable()
    {
        // 단어 데이터가 설정될 때까지 잠시 대기 후 확인
        StartCoroutine(CheckIsLearnedDelayed());
    }

    // 단어 데이터가 설정된 후 학습 여부 확인
    IEnumerator CheckIsLearnedDelayed()
    {
        // 프레임을 몇 번 기다려서 단어 데이터가 설정될 시간을 줌
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        CheckIsLearned();
        SetUI();
    }

    // 외부에서 단어가 설정된 후 호출할 수 있는 public 메서드
    public void CheckLearningStatus()
    {
        CheckIsLearned();
        SetUI();
    }

    // 이미 학습한 적이 있는가 확인
    void CheckIsLearned()
    {
        Box_word boxWord = this.GetComponent<Box_word>();
        
        if (boxWord != null)
        {
            string id = boxWord.id.ToString();
            string filepath = Application.persistentDataPath + "/" + WordManager.Instance.m_section + "_" + id + ".png";

            FileInfo fileInfo = new FileInfo(filepath);

            if (fileInfo.Exists) // 그린 적 있음
            {
                isLearned = true;
            }
            else // 그린 적이 없음
            {
                isLearned = false;
            }
        }
        else
        {
            // Box_word 컴포넌트가 없거나 id가 설정되지 않은 경우
            isLearned = false;
        }
    }

    void SetUI()
    {
        if (isLearned)
        {
            medal.SetActive(true);
        }
        else 
        {
            medal.SetActive(false);
        }
    }
}
