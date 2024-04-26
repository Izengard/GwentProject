using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Leader : MonoBehaviour, IPointerEnterHandler
{
    [field: SerializeField] public LeaderInfo LeaderInfo { get; private set; }
    [SerializeField] Image Artwork;
    [SerializeField] Image FactionLogo;
    [SerializeField] Button LeaderButton;
    [SerializeField] Player ownerPlayer;
    bool effectIsAvailable = true;

    void Start()
    {
        gameObject.name = LeaderInfo.name;
        Artwork.sprite = LeaderInfo.Artwork;
        FactionLogo.sprite = LeaderInfo.FactionLogo;
    }

    public void SetLeaderInfo(LeaderInfo LeaderInfo)
    {
        this.LeaderInfo = LeaderInfo;
    }

    public void ActivateEffect()
    {
        effectIsAvailable = false;
        Debug.Log($"LeaderEffect");
        CardManager.Instance.ActivateLeaderEffect(this);
        GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, 1);
    }

    public void ResetEffect() => effectIsAvailable = true;
    public void OnPointerEnter(PointerEventData eventData)
    {
        InfoDisplay.Instance.DisplayCardInfo(this.LeaderInfo);
    }

    void Update()
    {
        LeaderButton.interactable = GameManager.Instance.currentPlayer == this.ownerPlayer
                                    && GameManager.Instance.CurrentTurnPhase == TurnPhase.Play
                                    && effectIsAvailable;
    }
}
