using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour
{
    private bool AvailableEffect = true;
    public void ActivateEffect(){
        Debug.Log($"LeaderEffect");
        AvailableEffect = false;
    }
    public void ResetEffect() => AvailableEffect = true;
}
