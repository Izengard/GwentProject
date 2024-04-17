using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FactionButton : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] Image buttonImage;

    void Update()
    {
        if (toggle.isOn) buttonImage.color = Color.green;
        else buttonImage.color = Color.white;
    }
}
