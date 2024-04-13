using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBoard : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private GameObject hand;
    public GameObject Hand => hand;
    [SerializeField] Leader Leader;


    public async void DealCards(int n)
    {
        for (int i = 0; i < n; i++)
        {
            var drawnCard = deck.Draw();
            LeanTween.move(drawnCard.gameObject, hand.transform.position, 1f)
            .setEaseOutQuad()
            .setOnComplete(() => drawnCard.transform.SetParent(hand.transform, false));

            int handCount = hand.gameObject.transform.childCount;
            if (handCount > 10 && GameManager.Instance.CurrentTurnPhase != TurnPhase.Draw)
                CardManager.Instance.SendToGraveyard(drawnCard);

            await Task.Delay(1200);
        }
    }

}
