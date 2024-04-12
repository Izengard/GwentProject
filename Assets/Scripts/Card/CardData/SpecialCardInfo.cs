using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
[CreateAssetMenu(fileName = "New Card", menuName = "Card/Special")]
public class SpecialCardInfo : CardInfo
{
    public SpecialType SpecialType;  
    public Sprite Type;

}
public enum SpecialType
{
    Buff, Rain, Fog, Blizzard, Decoy, Clearing
}

