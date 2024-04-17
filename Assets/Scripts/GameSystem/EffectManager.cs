using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EffectManager : MonoBehaviour
{
    GameBoard gameBoard => GameBoard.Instance;
    Player currentPlayer => GameManager.Instance.currentPlayer;
    int enemyPlayer => ((int)currentPlayer + 1) % 2;
    Dictionary<Card, Row>[] SummonedCardsByRow => CardManager.Instance.CardsOnPlayerField;


    public void ActivateEffect(Effect effect)
    {
        switch (effect)
        {
            case Effect.Draw:
                gameBoard.DealCards(currentPlayer, 1);
                break;
            case Effect.DestroyGreaterUnit:
                DestroyGreaterUnit();
                break;
            case Effect.DestroyLesserUnit:
                DestroyLesserUnit();
                break;
            case Effect.DestroyLesserRow:
                DestroyLesserRow();
                break;
            case Effect.BalanceFieldPower:
                BalanceFieldPower();
                break;
            case Effect.MultiplyPower:
                MultiplyPower();
                break;
            case Effect.SetBuff:
                SetBuff();
                break;
            case Effect.SetWeather:
                SetWeather();
                break;
        }
    }

    void DestroyGreaterUnit()
    {
        int maxPower = 0;
        foreach (Dictionary<Card, Row> summonedCards in SummonedCardsByRow)
            foreach (Card card in summonedCards.Keys)
                if (card is SilverUnit silverUnit)
                    maxPower = Math.Max(maxPower, silverUnit.Power);

        foreach (var summonedCards in SummonedCardsByRow)
            for (int i = summonedCards.Keys.Count - 1; i >= 0; i--)
            {
                var unit = summonedCards.Keys.ElementAt(i);
                if (unit is SilverUnit silverUnit && silverUnit.Power == maxPower)
                    DestroyUnitFrom((Unit)unit, summonedCards);
            }
    }

    void DestroyLesserUnit()
    {
        int minPower = int.MaxValue;
        var enemyCards = SummonedCardsByRow[enemyPlayer];
        foreach (Card card in enemyCards.Keys)
            if (card is SilverUnit silverUnit)
                minPower = Math.Min(minPower, silverUnit.Power);

        for (int i = enemyCards.Count - 1; i >= 0; i--)
        {
            var unit = enemyCards.Keys.ElementAt(i);
            if (unit is SilverUnit silverUnit && silverUnit.Power == minPower)
                DestroyUnitFrom((Unit)unit, enemyCards);
        }
    }

    void DestroyLesserRow()
    {
        // Determinate the min count
        int minUnitCount = int.MaxValue;
        foreach (Dictionary<Card, Row> summonedCards in SummonedCardsByRow)
            foreach (Row row in summonedCards.Values)
                minUnitCount = Math.Min(minUnitCount, row.UnitsCount);
        print($"minUnitCount {minUnitCount}");

        // Destroy all the the silver units from the rows wich UnitCount equals to minCount
        foreach (Dictionary<Card, Row> summonedCards in SummonedCardsByRow)
        {
            var activeRows = summonedCards.Values.ToArray();
            for (int i = activeRows.Length - 1; i >= 0; i--)
            {
                var row = summonedCards.Values.ElementAt(i);
                if (row.UnitsCount == minUnitCount)
                    for (int j = 0; j < row.UnitsCount; j++)
                        if (row.rowUnits[j] is SilverUnit silver)
                            DestroyUnitFrom(silver, summonedCards);

                row.ResetDecoys();
            }
        }
    }
    void BalanceFieldPower()
    {
        int average = 0, count = 0;
        foreach (var summonedCards in SummonedCardsByRow)
            foreach (var card in summonedCards.Keys)
                if (card is Unit unit) { average += unit.Power; count++; }

        average /= count;

        foreach (var summonedCards in SummonedCardsByRow)
            foreach (var card in summonedCards.Keys)
                if (card is SilverUnit silverUnit)
                    silverUnit.Power = average;
    }
    void MultiplyPower()
    {
        var cardName = SummonedCardsByRow[(int)currentPlayer].Last().Key.CardInfo.Name;
        Debug.Log($"{cardName}");

        var row = SummonedCardsByRow[(int)currentPlayer].Last().Value;
        int count = 0;

        foreach (var unit in row.rowUnits)
            if (unit.CardInfo.Name == cardName)
            {
                count++;
                Debug.Log($"{unit.CardInfo.Name}");
            }

        foreach (var unit in row.rowUnits)
            if (unit is SilverUnit silver && unit.CardInfo.Name == cardName)
                silver.Power = count * unit.UnitCardInfo.Power;
    }
    void SetBuff()
    {
        var row = SummonedCardsByRow[(int)currentPlayer].Last().Value;
        row.ActivateBuff();
    }
    void SetWeather()
    {
        var row = SummonedCardsByRow[(int)currentPlayer].Last().Value;
        int weatherIndex = (int)row.AttackType;
        gameBoard.SetWeather((GameBoard.Weather)weatherIndex);
    }

    /// <summary>
    /// When a card is to be destroyed it must be removed from the row, from the CardsOnField Dict 
    /// and send To graveyard This method encapsulate and abstract such process
    /// </summary>
    /// <param name="card">Card to be destroyed in the game</param>
    /// <param name="CardsOnField"></param> <summary>
    void DestroyUnitFrom(Unit unit, Dictionary<Card, Row> CardsOnField)
    {
        var row = CardsOnField[unit];
        CardsOnField.Remove(unit);
        row.RemoveUnit(unit);
        CardManager.Instance.SendToGraveyard(unit);
    }
}
