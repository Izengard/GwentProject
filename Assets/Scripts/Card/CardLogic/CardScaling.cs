using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardControls : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {     
        this.transform.localScale = Vector3.one;
    }
}
