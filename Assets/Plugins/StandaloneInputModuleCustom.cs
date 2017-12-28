using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StandaloneInputModuleCustom : StandaloneInputModule {

    public void GetCurPointer()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData eventData;
            GetPointerData(-1, out eventData, false);
            if (eventData != null && eventData.pointerPress != null)
            {
                //播放音乐
            }
        }
#else
     if (Input.touchCount > 0)
    {
            Touch touch = Input.GetTouch(0);
            bool pressed;
            bool released;
            PointerEventData eventData = GetTouchPointerEventData(touch, out pressed, out released);
            if (eventData != null && eventData.pointerPress != null)
            {
                //播放音乐
            }
    }
#endif
}
}
