using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardInfo : ScriptableObject
{
    public string Faction;
    public Sprite FactionLogo;
    public string Name;
    public Effect effect;
    public Sprite Artwork;
    public string Description;
}

public enum Effect
{
    Draw, DestroyLesserRow, DestroyWeakestUnit, DestroyStrongestUnit, CoordinatedTactics,
    BalanceFieldPower, Buff, Weather, Versatile, None, Decoy, Clearing
}