using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : ScriptableObject
{
    public string Faction;
    public Sprite FactionLogo;
    public string Name;
    public Effect effect;
    public string Effect => effect.ToString();
    public Sprite Artwork;
    public string Description;
}


public enum Effect
{
    Draw, DestroyLesserRow, DestroyLesserUnit, DestroyGreaterUnit,
    MultiplyPower, BalanceFieldPower, SetBuff, SetWeather, Versatile, Null,Decoy
}