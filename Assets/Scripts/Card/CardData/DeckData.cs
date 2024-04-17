using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDataBase", menuName = "Card/Deck")]
public class DeckData : ScriptableObject
{
   public List<CardInfo> CardList;
}
