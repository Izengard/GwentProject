using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardDataBase", menuName = "Card/CardsDataBase")]
public class CardDataBase : ScriptableObject
{
   public List<CardInfo> CardsDB;
   public List<DeckData> DecksDB;
   public List<LeaderInfo> LeadersDB;
}
