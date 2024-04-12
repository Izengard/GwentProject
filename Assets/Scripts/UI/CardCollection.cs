using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    [SerializeField] CardGenerator cardGenerator;
    [SerializeField] Transform[] FactionGrid;
    [SerializeField] CardDB[] Factions = new CardDB[4];

    void Start()
    {
        for (int i = 0; i < Factions.Length; i++)
            for (int j = 0; j < Factions[i].CardList.Count; j++)
            {
                var cardInfo = Factions[i].CardList[j];
                var card = cardGenerator.InstantiateCard(cardInfo);
                card.transform.SetParent(FactionGrid[i]);
            }
    }
}
