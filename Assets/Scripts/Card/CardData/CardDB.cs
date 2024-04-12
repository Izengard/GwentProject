using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDataBase", menuName = "Card/CardsDB")]
public class CardDB : ScriptableObject
{
   public List<CardInfo> CardList;
}
