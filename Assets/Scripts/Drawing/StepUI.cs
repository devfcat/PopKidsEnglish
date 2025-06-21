using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepUI : MonoBehaviour
{
    private int _step = 0;
    public int step 
    { 
        get { return _step; }
        set 
        { 
            if (_step != value)
            {
                _step = value;
                SetStep();
            }
        }
    }
    public List<GameObject> ui_steps;

    void OnEnable()
    {
        step = 0;
    }

    public void SetStep()
    {
        // step 값에 따라 UI 업데이트
        for (int i = 0; i < ui_steps.Count; i++)
        {
            if (i == step)
            {
                ui_steps[i].SetActive(true);
            }
            else ui_steps[i].SetActive(false);
        }
    }
}
