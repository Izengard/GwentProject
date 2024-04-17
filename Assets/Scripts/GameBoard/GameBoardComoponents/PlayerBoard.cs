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
    [SerializeField] CardDataBase dataBase;


    public void SetPlayerFaction(int playerNumber)
    {
        int[] selectedFactions = new int[2];
        selectedFactions[0] = PlayerPrefs.GetInt("P1Faction",0);
        selectedFactions[1] = PlayerPrefs.GetInt("P2Faction",1);

        var selectedFaction = selectedFactions[playerNumber];
        var leaderInfo = dataBase.LeadersDB[selectedFaction];
        Leader.SetLeaderInfo(leaderInfo);

        var deckData = dataBase.DecksDB[selectedFaction];
        deck.SetDeckData(deckData);
    }
    
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

            await Task.Delay(1000);
        }
    }


    public void ResetLeaderEffect() => Leader.ResetEffect();

}
