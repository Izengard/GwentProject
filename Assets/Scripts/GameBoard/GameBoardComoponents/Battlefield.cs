using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    [SerializeField] Row[] rows;
    public Row[] Rows => rows;
    public Row MeleeRow => Rows[0];
    public Row RangedRow => Rows[1];
    public Row SiegeRow => Rows[2];

    public Row this[AttackType attack] => Rows[(int)attack];

    public int FieldPower
    {
        get
        {
            int totalPower = 0;
            foreach (var row in rows)
                totalPower += row.PowerSubtotal;
            
            return totalPower;
        }
    }

    public void ResetField()
    {
        foreach (var row in rows)
            row.ResetRow();
    }
    public void HighlightRows(Card pendingCard)
    {
        if (pendingCard is Unit unit)
            foreach (var type in unit.UnitCardInfo.AttackTypes)
            {
                int RowIndex = (int)type;
                Rows[RowIndex].HighlightOn();
            }
        else if (pendingCard is SpecialCard buff)
        {
            foreach (var row in rows)
                if (!row.BuffIsActive) row.HighlightOn();
        }
    }

    public void HighlightRowsOff()
    {
        foreach (var row in rows)
            row.HighlightOff();
    }
}

