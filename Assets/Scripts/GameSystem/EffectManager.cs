using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EffectManager : MonoBehaviour
{
    [SerializeField] GameBoard gameBoard;
    Player currentPlayer => GameManager.Instance.currentPlayer;
    int enemyPlayer => ((int)currentPlayer + 1) % 2;
    Dictionary<Card, Row>[] CardsOnField => CardManager.Instance.CardsOnPlayerField;
    Battlefield currentPlayerField => gameBoard.PlayerBattlefield[(int)currentPlayer];
    Battlefield enemyPlayerField => gameBoard.PlayerBattlefield[(int)currentPlayer];

    public void ActivateUnitEffect(Unit unit)
    {
        switch (unit.card.effect)
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
                // DestroyLesserRow();
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
        for (int i = 0; i < CardsOnField.Length; i++)
            foreach (var card in CardsOnField[i].Keys)
                if (card is SilverUnit silverUnit)
                    maxPower = Math.Max(maxPower, silverUnit.Power);

        for (int i = 0; i < CardsOnField.Length; i++)
            foreach (var card in CardsOnField[i].Keys)
                if (card is SilverUnit silverUnit && silverUnit.Power == maxPower)
                {
                    CardManager.Instance.SendToGraveyard(silverUnit);
                    LeanTween.delayedCall(.5f,
                    () => CardsOnField[i].Remove(card));
                }
    }

    void DestroyLesserUnit()
    {
        int minPower = int.MaxValue;
        foreach (var card in CardsOnField[enemyPlayer].Keys)
            if (card is SilverUnit silverUnit)
                minPower = Math.Min(minPower, silverUnit.Power);

        foreach (var card in CardsOnField[enemyPlayer].Keys)
            if (card is SilverUnit silverUnit && silverUnit.Power == minPower)
            {
                CardManager.Instance.SendToGraveyard(silverUnit);
                LeanTween.delayedCall(.5f, () =>
                            CardsOnField[enemyPlayer].Remove(card));
            }
    }

    void DestroyLesserRow()
    {
        // Determinate the min count
        int minUnitCount = int.MaxValue;
        foreach (var field in new[] { currentPlayerField, enemyPlayerField })
            foreach (var row in field.Rows)
                minUnitCount = Math.Min(minUnitCount, row.UnitsCount);

        // Destroy all the rows with UnitCount equals to minCount
        foreach (var field in new[] { currentPlayerField, enemyPlayerField })
            foreach (var row in field.Rows)
                if (row.UnitsCount == minUnitCount)
                {
                    foreach (var card in row.rowUnits)
                        if (card is SilverUnit silver)
                            CardManager.Instance.SendToGraveyard(silver);

                    row.DestroyUnits();
                }

        // UpdateCardManagerDataBase
        foreach (var field in CardsOnField)
            for (int i = field.Count - 1; i >= 0; i--)
            {
                var pair = field.ElementAt(i);
                if (pair.Value.UnitsCount == minUnitCount)
                    field.Remove(pair.Key);
            }
    }
    void BalanceFieldPower()
    {
        int average = 0, count = 0;
        foreach (var field in CardsOnField)
            foreach (var card in field.Keys)
                if (card is Unit unit)
                {
                    average += unit.Power;
                    count++;
                }

        average /= count;

        foreach (var field in CardsOnField)
            foreach (var card in field.Keys)
                if (card is SilverUnit silverUnit)
                    silverUnit.Power = average;
    }
    void MultiplyPower()
    {
        var cardName = CardsOnField[(int)currentPlayer].Last().Key.CardInfo.Name;
        Debug.Log($"{cardName}");

        var row = CardsOnField[(int)currentPlayer].Last().Value;
        int count = 0;

        foreach (var unit in row.rowUnits)
            if (unit.CardInfo.Name == cardName)
            {
                count++;
                Debug.Log($"{unit.CardInfo.Name}");
            }

        foreach (var unit in row.rowUnits)
            if (unit is SilverUnit silver && unit.CardInfo.Name == cardName)
                silver.MultiplyPower(count);
    }
    void SetBuff()
    {
        var row = CardsOnField[(int)currentPlayer].Last().Value;
        row.ActivateBuff();
    }
    void SetWeather()
    {
        var row = CardsOnField[(int)currentPlayer].Last().Value;
        int weatherIndex = (int)row.AttackType;
        gameBoard.SetWeather((GameBoard.Weather)weatherIndex);
    }
}
