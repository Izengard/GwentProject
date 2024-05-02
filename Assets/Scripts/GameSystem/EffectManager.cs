using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EffectManager : MonoBehaviour
{
    Player currentPlayer => GameManager.Instance.currentPlayer;
    int enemyPlayer => ((int)currentPlayer + 1) % 2;
    Dictionary<Card, Row> SummonedCardsByRow => CardManager.Instance.SummonedCardsByRow;
    List<SpecialCard> WeathersByPlayer => CardManager.Instance.ActiveWeathers;
    Action<Unit> DestroyUnit => CardManager.Instance.DestroyUnit;

    public void ActivateUnitEffect(Unit unit) => ActivateEffect(unit.CardInfo.effect, unit);
    public void ActivateLeaderEffect(Leader leader) => ActivateEffect(leader.LeaderInfo.effect);

    public void ActivateEffect(Effect effect, Unit unit = null)
    {
        switch (effect)
        {
            case Effect.Draw:
                GameBoard.Instance.DealCards(currentPlayer, 1);
                break;
            case Effect.DestroyStrongestUnit:
                DestroyStrongestUnit();
                break;
            case Effect.DestroyWeakestUnit:
                DestroyLesserUnit();
                break;
            case Effect.DestroyLesserRow:
                DestroyLesserRow();
                break;
            case Effect.BalanceFieldPower:
                BalanceFieldPower();
                break;
            case Effect.CoordinatedTactics:
                MultiplyPower(unit);
                break;
            case Effect.Buff:
                var row = SummonedCardsByRow[unit];
                row.ActivateBuff();
                break;
            case Effect.Weather:
                row = SummonedCardsByRow[unit];
                var weather = (Weather)(int)row.AttackType;
                GameBoard.Instance.SetWeather(weather);
                break;
            case Effect.Clearing:
                Clearing();
                break;
        }
    }

    void DestroyStrongestUnit()
    {
        var silverUnits = SummonedCardsByRow.Keys
                        .OfType<SilverUnit>() // Filters and casts to SilverUnit
                        .ToArray();             

        int maxPower = 0;
        foreach (SilverUnit unit in silverUnits)
            maxPower = Math.Max(maxPower, unit.Power);

        foreach (SilverUnit unit in silverUnits)
            if (unit.Power == maxPower)
                DestroyUnit(unit);
    }

    void DestroyLesserUnit()
    {
        var enemyUnits = SummonedCardsByRow.Keys
                        .OfType<SilverUnit>()
                        .Where(card => card.Owner == (Player)enemyPlayer)
                        .ToArray();

        int minPower = int.MaxValue;
        foreach (var unit in enemyUnits)
            minPower = Math.Min(minPower, unit.Power);

        foreach (SilverUnit unit in enemyUnits)
            if (unit.Power == minPower)
                DestroyUnit(unit);
    }

    void DestroyLesserRow()
    {
        // Determinate the min count
        int minUnitCount = int.MaxValue;
        foreach (Row row in SummonedCardsByRow.Values)
            minUnitCount = Math.Min(minUnitCount, row.UnitsCount);

        // Destroy all the the silver units from the rows whose UnitCount equals minCount
        var activeRows = SummonedCardsByRow.Values.ToArray();
        for (int i = 0; i < activeRows.Length; i++)
        {
            if (activeRows[i].UnitsCount == minUnitCount)
                activeRows[i].DestroyAllSilverUnits();
        }
    }

    void BalanceFieldPower()
    {
        int average = 0, count = 0;
        foreach (var card in SummonedCardsByRow.Keys)
            if (card is Unit unit) { average += unit.Power; count++; }

        if (count == 0) return;
        average /= count;

        foreach (var card in SummonedCardsByRow.Keys)
            if (card is SilverUnit silverUnit)
                silverUnit.Power = average;
    }
    void MultiplyPower(Unit unit)
    {
        var cardName = unit.CardInfo.Name;

        var row = SummonedCardsByRow[unit];
        int count = 0;

        foreach (var rowUnit in row.rowUnits)
            if (rowUnit.CardInfo.Name == cardName)
                count++;

        foreach (var rowUnit in row.rowUnits)
            if (rowUnit is SilverUnit silver && silver.CardInfo.Name == cardName)
                silver.Power = count * unit.UnitCardInfo.Power;
    }

    public void Clearing()
    {
        GameBoard.Instance.ResetWeather();
        foreach (var weather in WeathersByPlayer)
            CardManager.Instance.SendToGraveyard(weather);
        Debug.Log($"Clearing Card Played");
    }
}
