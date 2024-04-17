using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leader : MonoBehaviour
{
    [SerializeField] LeaderInfo leaderInfo;
    [SerializeField] Image Artwork;
    [SerializeField] Image FactionLogo;
    [SerializeField] Button LeaderButton;


    void Start()
    {
        gameObject.name = leaderInfo.name;
        Artwork.sprite = leaderInfo.Artwork;
        FactionLogo.sprite = leaderInfo.FactionLogo;
    }

    public void SetLeaderInfo(LeaderInfo leaderInfo)
    {
        this.leaderInfo = leaderInfo;
    }

    public void ActivateEffect()
    {
        if (GameManager.Instance.CurrentTurnPhase != TurnPhase.Play)
           return; 
        Debug.Log($"LeaderEffect");
        CardManager.Instance.ActivateEffect(leaderInfo.effect);
        LeaderButton.interactable = false;
        GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd);
    }

    public void ResetEffect() => LeaderButton.interactable = true;
}
