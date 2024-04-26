using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<CardInfo> DeckCards;
    [SerializeField] private CardGenerator cardGenerator;
    [SerializeField] AudioSource SoundEffect;
    Player ownerPlayer;

    public void SetDeckData(DeckData deckData)
    {
        DeckCards = new List<CardInfo>(deckData.CardList);
        Shuffle();
    }

    public void SetOwnerPlayer(Player owner) => this.ownerPlayer = owner;


    public void Shuffle()
    {
        int n = DeckCards.Count() - 1;
        while (n >= 1)
        {
            int newPos = Random.Range(0, n);
            var temp = DeckCards[newPos];
            DeckCards[newPos] = DeckCards[n];
            DeckCards[n--] = temp;
        }
    }

    public Card Draw()
    {
        if (DeckCards.Count == 0) return null;

        SoundEffect.Play();
        var cardInfo = DeckCards.Last();
        DeckCards.RemoveAt(DeckCards.Count - 1);
        var drawnCard = cardGenerator.InstantiateCard(cardInfo);
        drawnCard.SetOwnerPlayer(this.ownerPlayer);
        return drawnCard;
    }

    public void ReAddCards(params CardInfo[] cards)
    {
        foreach (var card in cards)
            if (card is not null) this.DeckCards.Add(card);

        Shuffle();
    }
}
