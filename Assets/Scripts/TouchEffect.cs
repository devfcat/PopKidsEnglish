using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// 화면을 터치하면 해당 위치에 터치 애니메이션을 보여주는 스크립트
public class TouchEffect : MonoBehaviour
{
    public GameObject effect;
    public Touch touch;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Stop_Pointers();

            int num = Random.Range(0,2);
            Vector2 pos = new Vector2(Input.mousePosition.x-Screen.width/2, Input.mousePosition.y-Screen.height/2);

            Play_Pointers_pc(pos);
        }
#endif
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                Stop_Pointers();
                Play_Pointers();
            }
        }
    }

    public void Play_Pointers() // 실제 적용되는 터치이펙트 실행 함수
    {
        SoundManager.Instance.PlaySFX(SFX.touch);

        effect.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(touch.position.x-Screen.width/2, touch.position.y-Screen.height/2);
        effect.SetActive(true);

        Invoke("Stop_Pointers", 1f);
    }

    public void Play_Pointers_pc(Vector2 pos) // 유니티 에디터에서의 터치이펙트 테스트용
    {
        SoundManager.Instance.PlaySFX(SFX.touch);

        effect.GetComponent<RectTransform>().anchoredPosition
            = pos;
        effect.SetActive(true);
        
        Invoke("Stop_Pointers", 1f);
    }

    public void Stop_Pointers() // 포인터 비활성화
    {
        effect.SetActive(false);
    }
}
