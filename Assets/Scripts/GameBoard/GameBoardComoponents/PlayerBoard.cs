using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        selectedFactions[0] = PlayerPrefs.GetInt("P1Faction", 0);
        selectedFactions[1] = PlayerPrefs.GetInt("P2Faction", 1);

        var selectedFaction = selectedFactions[playerNumber];
        var leaderInfo = dataBase.LeadersDB[selectedFaction];
        Leader.SetLeaderInfo(leaderInfo);

        var deckData = dataBase.DecksDB[selectedFaction];
        deck.SetDeckData(deckData);
        deck.SetOwnerPlayer((Player)playerNumber);
    }

    public void DealCards(int n)
    {
        for (int i = 0; i < n; i++)
        {
            var drawnCard = deck.Draw();
            int handCount = hand.gameObject.transform.childCount;
            
            if(GameManager.Instance.CurrentTurnPhase  == TurnPhase.Draw)
                drawnCard.transform.SetParent(Hand.transform);
            else
                CardManager.Instance.MoveCardTo(drawnCard, Hand.transform);

            if (handCount >= 10 && GameManager.Instance.CurrentTurnPhase != TurnPhase.Draw)
                CardManager.Instance.SendToGraveyard(drawnCard);
            drawnCard.transform.localScale = Vector3.one;
        }
    }


    public void ResetLeaderEffect() => Leader.ResetEffect();

}
