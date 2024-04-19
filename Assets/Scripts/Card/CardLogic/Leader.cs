using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Leader : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] LeaderInfo leaderInfo;
    [SerializeField] Image Artwork;
    [SerializeField] Image FactionLogo;
    [SerializeField] Button LeaderButton;
    [SerializeField] Player ownerPlayer;
    bool effectIsAvailable = true;

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
        effectIsAvailable = false;   
        Debug.Log($"LeaderEffect");
        CardManager.Instance.ActivateEffect(leaderInfo.effect);
        GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd,1);
    }

    public void ResetEffect() => effectIsAvailable = true;
    public void OnPointerEnter(PointerEventData eventData)
    {
        InfoDisplay.Instance.DisplayCardInfo(this.leaderInfo);
    }

    void Update()
    {
        LeaderButton.interactable = GameManager.Instance.currentPlayer == this.ownerPlayer
                                    && GameManager.Instance.CurrentTurnPhase == TurnPhase.Play
                                    && effectIsAvailable;
    }
}
