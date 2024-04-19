using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] Toggle[] factionButtons = new Toggle[3];
    [SerializeField] TMP_InputField nameInput;


    public void DisableInteractions()
    {
        nameInput.interactable = false;
        foreach (var button in factionButtons)
        {
            button.interactable = false;
        }
    }



}
