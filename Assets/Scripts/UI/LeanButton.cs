using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.1f), 0.5f).setEaseOutBounce();

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBounce();

    }

}
