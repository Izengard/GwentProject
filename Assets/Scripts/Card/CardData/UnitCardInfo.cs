using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Card", menuName = "Card/Unit")]
public class UnitCardInfo : CardInfo
{
    public UnitType UnitType;
    public int Power;
    public Attack[] AttackTypes;
}

public enum UnitType { Silver, Golden }

public enum Attack
{
    Melee, Ranged, Siege
}
