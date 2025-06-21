using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 내 단어장 보기 시 선택한 분류를 참조하도록 설정한 스크립트
/// </summary>
public class LearnSelect_Manager : MonoBehaviour
{
    public string m_selected_section;

    // 싱글톤 인스턴스
    private static LearnSelect_Manager _instance;
    public static LearnSelect_Manager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(LearnSelect_Manager)) as LearnSelect_Manager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        m_selected_section = "";
    }
}
