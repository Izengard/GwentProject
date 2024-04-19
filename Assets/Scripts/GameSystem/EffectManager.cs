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
    Dictionary<SpecialCard, Player> WeathersByPlayer => CardManager.Instance.WeathersByPlayer;


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
            case Effect.Clearing:
                Clearing();
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

        for (int i = 0; i < SummonedCardsByRow.Length; i++)
        {
            var summonedCards = SummonedCardsByRow[i];
            for (int j = summonedCards.Keys.Count - 1; j >= 0; j--)
            {
                var unit = summonedCards.Keys.ElementAt(j);
                if (unit is SilverUnit silverUnit && silverUnit.Power == maxPower)
                    DestroyUnitFrom((Unit)unit, summonedCards, (Player)i);
            }
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
                DestroyUnitFrom((Unit)unit, enemyCards, (Player)enemyPlayer);
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
        for (int i = 0; i < SummonedCardsByRow.Length; i++)
        {
            Dictionary<Card, Row> summonedCards = SummonedCardsByRow[i];
            var activeRows = summonedCards.Values.ToArray();
            for (int j = activeRows.Length - 1; j >= 0; j--)
            {
                var row = summonedCards.Values.ElementAt(j);
                if (row.UnitsCount == minUnitCount)
                {
                    Debug.Log($"{row.UnitsCount}");
                    for (int k = 0; k < row.UnitsCount; k++)
                        if (row.rowUnits[k] is SilverUnit silver)
                            DestroyUnitFrom(silver, summonedCards, (Player)i);

                    row.ResetDecoys();
                }
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
    public void Clearing()
    {
        gameBoard.ResetWeather();
        foreach (var weather in WeathersByPlayer)
            CardManager.Instance.SendToGraveyard(weather.Key, weather.Value);
        Debug.Log($"Clearing Card Played");
    }

    /// <summary>
    /// When a card is to be destroyed it must be removed from the row, from the CardsOnField Dict 
    /// and send To graveyard This method encapsulate and abstract such process
    /// </summary>
    /// <param name="card">Card to be destroyed in the game</param>
    /// <param name="CardsOnField"></param> Dictionary in which the card's reference is kept<summary>
    /// <param name="player"></param> Player to whom graveyard the card will be sent to <summary>
    void DestroyUnitFrom(Unit unit, Dictionary<Card, Row> CardsOnField, Player player)
    {
        var row = CardsOnField[unit];
        CardsOnField.Remove(unit);
        row.RemoveUnit(unit);
        CardManager.Instance.SendToGraveyard(unit, player);
    }
}
