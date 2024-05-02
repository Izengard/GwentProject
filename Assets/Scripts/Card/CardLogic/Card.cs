using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    protected bool cardIsInHand = true;
    public abstract CardInfo CardInfo { get; }
    public abstract void SetCardInfo(CardInfo cardInfo);
    public Player Owner { get; set; }
    public void SetOwnerPlayer(Player player) => Owner = player;


    public void OnPointerClick(PointerEventData eventData)
    {

        if (GameManager.Instance.CurrentTurnPhase == TurnPhase.Play && cardIsInHand)
        {
            cardIsInHand = false;
            GameManager.Instance.UpdateTurnPhase(TurnPhase.Summon);
            CardManager.Instance.SummonCard(this);
            Debug.Log(GameManager.Instance.CurrentTurnPhase);
        }
        else if (GameManager.Instance.GameState == GameState.Start)
            InitialHandPanel.Instance.ChangeThisCard(this);

    }

    public void OnPointerEnter(PointerEventData eventData)
        => InfoDisplay.Instance.DisplayCardInfo(this.CardInfo);

    public void SetBackToHand() => cardIsInHand = true;
}
