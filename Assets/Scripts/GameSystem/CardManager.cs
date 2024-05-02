using static LeanTween;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class CardManager
{
   public static CardManager Instance { get; private set; } // Singleton CardManager
   [SerializeField] private Transform[] graveyards;
   [SerializeField] private EffectManager effectManager;
   Player currentPlayer => GameManager.Instance.currentPlayer;
   GameBoard gameBoard => GameBoard.Instance;
   Battlefield currentField => gameBoard.PlayerBattlefields[(int)currentPlayer];
   [SerializeField] Button CancelButton;
   [SerializeField] float cardMoveDuration = 0.8f;

   // Let's keep a reference of all the cards that has been summoned so we can quickly manage them
   public Dictionary<Card, Row> SummonedCardsByRow { get; private set; } = new Dictionary<Card, Row>();
   public List<SpecialCard> ActiveWeathers { get; private set; } = new();
   Card pendingCard;


   void Awake()
   {
      if (Instance == null) Instance = this;
      else if (Instance != this) Destroy(gameObject);
   }

   public void SummonCard(Card card)
   {
      if (GameManager.Instance.CurrentTurnPhase != TurnPhase.Summon) return;

      PlayCardAudio(card);

      if (card is Unit unit)
         SummonUnitCard(unit);
      else if (card is SpecialCard special)
         SummonSpecialCard(special);
   }

   void SummonUnitCard(Unit unit)
   {
      var attacks = unit.UnitCardInfo.AttackTypes;
      if (attacks.Length == 1)
      {
         var destinationRow = currentField[attacks[0]];
         SummonUnitInRow(unit, destinationRow);
      }
      else
      {
         // We let the selected card pending, highlight the rows this card may be summoned to,
         // and wait for the player to click one of them, then we will summon the card to that row.
         // with the HandleRowSelection method (see Below)
         AskForPlayerInput(unit);
         GameManager.Instance.WaitForRowSelection();
      }
   }

   void SummonSpecialCard(SpecialCard special)
   {
      switch (special.SpecialCardInfo.SpecialType)
      {
         case SpecialType.Clearing:
            SendToGraveyard(special);
            effectManager.ActivateEffect(Effect.Clearing);
            GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, cardMoveDuration);
            break;
         case SpecialType.Buff:
            AskForPlayerInput(special);
            GameManager.Instance.WaitForRowSelection();
            break;
         case SpecialType.Decoy:
            Debug.Log($"DecoyActivated");
            if (!AnySilverUnit())
            {
               Debug.Log(!AnySilverUnit());
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }
            AskForPlayerInput(special);
            GameManager.Instance.WaitForCardSelection();
            break;
         default:
            SummonWeather(special);
            break;
      }
   }

   void SummonUnitInRow(Unit unit, Row row)
   {
      PlayCardAudio(unit);
      row.AddUnit(unit);
      SummonedCardsByRow.Add(unit, row);
      MoveCardTo(unit, row.RowUnitsTransform);
      CheckRowPowerMods(unit, row);

      float effectTimeDelay = cardMoveDuration + .1f;
      if (unit.CardInfo.effect == Effect.None || unit.CardInfo.effect == Effect.Versatile
         || unit.CardInfo.effect == Effect.CoordinatedTactics || unit.CardInfo.effect == Effect.Buff)
      {
         effectTimeDelay = 0f;
      }

      delayedCall(effectTimeDelay, () =>
         ActivateEffect(unit));
      GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, effectTimeDelay + cardMoveDuration);
   }

   void SummonWeather(SpecialCard special)
   {
      Weather weather;
      Transform newPosition;
      if (special.SpecialCardInfo.SpecialType == SpecialType.Blizzard)
      {
         weather = Weather.Blizzard;
         newPosition = gameBoard.Weathers.Blizzard.transform;
      }
      else if (special.SpecialCardInfo.SpecialType == SpecialType.Fog)
      {
         weather = Weather.Fog;
         newPosition = gameBoard.Weathers.Fog.transform;
      }
      else //if(special.SpecialCardInfo.SpecialType == SpecialType.Rain)
      {
         weather = Weather.Rain;
         newPosition = gameBoard.Weathers.Rain.transform;
      }

      if (gameBoard.IsWeatherActive(weather))
      {
         GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
         return;
      }

      MoveCardTo(special, newPosition);
      ActiveWeathers.Add(special);
      gameBoard.SetWeather(weather);
      GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, cardMoveDuration);
   }

   public void HandleRowSelection(Row row)
   {
      HighlightCardOff(pendingCard);
      currentField.HighlightRowsOff();
      CancelButton.gameObject.SetActive(false);

      if (pendingCard is Unit unit)
         SummonUnitInRow(unit, row);

      else if (pendingCard is SpecialCard buff)
      {
         MoveCardTo(buff, row.BuffTransform);
         SummonedCardsByRow.Add(buff, row);
         row.ActivateBuff();
         GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, cardMoveDuration);
      }
      pendingCard = null;
   }

   public void HandleDecoyTarget(SilverUnit unit)
   {
      if (unit.Owner != currentPlayer)
      {
         GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
         return;
      }

      unit.ReturnToHand();
      HighlightCardOff(pendingCard);
      HighlightSilverUnits(false);
      CancelButton.gameObject.SetActive(false);

      var row = SummonedCardsByRow[unit];
      row.RemoveUnit(unit);
      SummonedCardsByRow.Remove(unit);
      SummonedCardsByRow.Add(pendingCard, row);
      row.AddDecoy();

      MoveCardTo(pendingCard, unit.transform.parent);
      var hand = gameBoard.PlayerBoards[(int)currentPlayer].Hand.transform;
      MoveCardTo(unit, hand);
      GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, cardMoveDuration);
   }
}