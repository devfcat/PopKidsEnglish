using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_VersionCheck : MonoBehaviour
{
    public GameObject m_loadingPanel;

    public void Start()
    {
        m_loadingPanel = this.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        bool isOn = WordManager.Instance.isLoading;

        // 로딩 중일 경우
        if (isOn && !m_loadingPanel.activeSelf)
        {
            m_loadingPanel.SetActive(true);
        }
        else
        {
            if (!m_loadingPanel.activeSelf)
            {
                m_loadingPanel.SetActive(false);
            }
        }
    }
}
