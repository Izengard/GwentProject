using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    protected bool cardIsInHand = true;
    public abstract CardInfo CardInfo { get; }

    public abstract void SetCardInfo(CardInfo cardInfo);

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
        {
            GameManager.Instance.InitialHandPanel.ChangeThisCard(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InfoDisplay.Instance.DisplayCardInfo(this.CardInfo);
    }
}
