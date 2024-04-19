using static LeanTween;
using System.Collections.Generic;
using UnityEngine;
using static GameBoard.Weather;
using System.Linq;

public class CardManager : MonoBehaviour
{
   // Singleton Pattern CardManager
   public static CardManager Instance { get; private set; }
   [SerializeField] private Transform[] graveyards;
   [SerializeField] private EffectManager effectManager;
   Player currentPlayer => GameManager.Instance.currentPlayer;
   GameBoard gameBoard => GameBoard.Instance;
   Battlefield currentField => gameBoard.PlayerBattlefields[(int)currentPlayer];
   float cardMoveDuration = 0.8f;


   // Let's keep a reference of all the cards that has been summoned so we can quickly manage them
   public Dictionary<Card, Row>[] CardsOnPlayerField { get; private set; } = new Dictionary<Card, Row>[2];
   public Dictionary<SpecialCard, Player> WeathersByPlayer { get; private set; } = new();
   Card pendingCard;

   void Awake()
   {
      if (Instance == null) Instance = this;
      else if (Instance != this) Destroy(gameObject);
      CardsOnPlayerField[0] = new();
      CardsOnPlayerField[1] = new();
   }

   /// <summary>
   /// Summons a card during the summon phase of the game.
   /// </summary>
   /// <param name="card">The card to be summoned.</param>
   public void SummonCard(Card card)
   {
      if (GameManager.Instance.CurrentTurnPhase != TurnPhase.Summon) return;

      card.GetComponent<AudioSource>().Play();
      if (card is SpecialCard special)
         SummonSpecialCard(special);
      else if (card is Unit unit)
         SummonUnitCard(unit);
   }

   void SummonUnitCard(Unit unit)
   {
      var attacks = unit.UnitCardInfo.AttackTypes;
      if (attacks.Length == 1)
      {
         var destinationRow = currentField[attacks[0]];
         SummonUnitCardInRow(unit, destinationRow);
      }
      else
      {
         // We let the selected card pending, highlight the rows this card may be summoned to,
         // and wait for the player to click one of them, 
         // action delegated to the HandleRowSelection method (see below)
         pendingCard = unit;
         HighlightCard(pendingCard);
         currentField.HighlightRows(pendingCard);
         GameManager.Instance.WaitForRowSelection();
      }
   }

   void SummonSpecialCard(SpecialCard special)
   {
      Transform newPosition;
      switch (special.cardInfo.SpecialType)
      {
         case SpecialType.Blizzard:
            if (gameBoard.IsWeatherActive(Blizzard))
            {
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }
            WeathersByPlayer.Add(special, currentPlayer);
            newPosition = gameBoard.Weathers.Blizzard.transform;
            gameBoard.SetWeather(Blizzard);
            Debug.Log($" Blizzard Card Summoned");
            break;

         case SpecialType.Fog:
            if (gameBoard.IsWeatherActive(Fog))
            {
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }
            WeathersByPlayer.Add(special, currentPlayer);
            newPosition = gameBoard.Weathers.Fog.transform;
            gameBoard.SetWeather(Fog);
            Debug.Log($"Fog Card Summoned");
            break;

         case SpecialType.Rain:
            if (gameBoard.IsWeatherActive(Rain))
            {
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }
            WeathersByPlayer.Add(special, currentPlayer);
            newPosition = gameBoard.Weathers.Rain.transform;
            gameBoard.SetWeather(Rain);
            Debug.Log($"Rain Card Summoned");
            break;

         case SpecialType.Clearing:
            newPosition = graveyards[(int)currentPlayer];
            effectManager.ActivateEffect(Effect.Clearing);
            break;

         case SpecialType.Buff:
            newPosition = null;
            pendingCard = special;
            HighlightCard(pendingCard);
            currentField.HighlightRows(pendingCard);
            GameManager.Instance.WaitForRowSelection();
            Debug.Log($"Buff Card Selected");
            break;

         case SpecialType.Decoy:
            if (!AnySilverUnit())
            {
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }

            pendingCard = special;
            HighlightCard(pendingCard);
            HighlightAllSilverUnits(true);
            GameManager.Instance.WaitForCardSelection();
            newPosition = null;
            Debug.Log($"Decoy Card Selected");
            break;

         default:
            newPosition = null;
            Debug.Log($"No Valid Card Type detected");
            break;
      }
      if (newPosition is not null)
      {
         MoveCardTo(special, newPosition);
         GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd);
      }
   }

   void SummonUnitCardInRow(Unit unit, Row row)
   {
      unit.GetComponent<AudioSource>().Play();
      row.AddUnit(unit);
      CardManager.Instance.CardsOnPlayerField[(int)currentPlayer].Add(unit, row);
      MoveCardTo(unit, row.RowUnitsTransform);
      CheckRowPowerMods(unit, row);

      float effectTimeDelay = .85f;
      if (unit.CardInfo.effect == Effect.Null || unit.CardInfo.effect == Effect.Versatile
         || unit.CardInfo.effect == Effect.MultiplyPower || unit.CardInfo.effect == Effect.SetBuff
         || unit.CardInfo.effect == Effect.SetWeather)
      { effectTimeDelay = 0f; }

      delayedCall(effectTimeDelay, () =>
         ActivateEffect(unit.CardInfo.effect));
      GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd,effectTimeDelay + cardMoveDuration);
   }

   public void ActivateEffect(Effect effect) => effectManager.ActivateEffect(effect);

   void HighlightAllSilverUnits(bool On)
   {
      foreach (var card in CardsOnPlayerField[(int)currentPlayer].Keys)
      {
         if (card is SilverUnit silver)
            if (On) HighlightCard(silver);
            else HighlightCardOff(silver);
      }
   }

   public void HandleRowSelection(Row row)
   {
      HighlightCardOff(pendingCard);
      currentField.HighlightRowsOff();
      if (pendingCard is Unit unit)
      {
         SummonUnitCardInRow(unit, row);
      }
      else if (pendingCard is SpecialCard buff)
      {
         MoveCardTo(buff, row.BuffTransform);
         CardsOnPlayerField[(int)currentPlayer].Add(buff, row);
         row.ActivateBuff();
         GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, cardMoveDuration);
      }
      pendingCard = null;
   }

   public void HandleDecoyTarget(SilverUnit unit)
   {
      if (!CardsOnPlayerField[(int)currentPlayer].Keys.Contains(unit))
      {
         GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
         return;
      }

      unit.ReturnToHand();
      HighlightCardOff(pendingCard);
      HighlightAllSilverUnits(false);

      var row = CardsOnPlayerField[(int)currentPlayer][unit];
      row.RemoveUnit(unit);
      CardsOnPlayerField[(int)currentPlayer].Remove(unit);
      CardsOnPlayerField[(int)currentPlayer].Add(pendingCard, row);
      row.AddDecoy();

      MoveCardTo(pendingCard, unit.transform.parent);
      var hand = gameBoard.PlayerBoards[(int)currentPlayer].Hand.transform;
      MoveCardTo(unit, hand);
      GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd, cardMoveDuration);
   }

   public void CancelCardSelection()
   {
      pendingCard = null;
      HighlightAllSilverUnits(false);
      currentField.HighlightRowsOff();
      GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
   }

   public bool AnySilverUnit()
   {
      foreach (var card in CardsOnPlayerField[(int)currentPlayer].Keys)
      {
         if (card is SilverUnit unit) return true;
      }
      return false;
   }

   public static void CheckRowPowerMods(Unit unit, Row row)
   {
      if (unit is SilverUnit silverUnit)
      {
         if (row.WeatherIsActive)
            silverUnit.SetWeather();
         else if (row.BuffIsActive)
            silverUnit.SetBuff();
      }
   }

   public void MoveCardTo(Card card, Transform newPosition)
   {
      if (newPosition is null) return;
      card.transform.SetParent(gameBoard.transform);
      LeanTween.move(card.gameObject, newPosition.position, cardMoveDuration)
               .setOnComplete(PutInside);

      void PutInside()
      {
         card.transform.position = newPosition.transform.position;
         card.transform.SetParent(newPosition.transform);
      }
   }

   public void SendToGraveyard(Card card, Player player)
   {
      var graveyardTransform = graveyards[(int)player];
      MoveCardTo(card, graveyardTransform);
   }


   // Highlight the pending card by increasing its local scale, disable its controls to 
   // avoid rescaling on pointer exit
   void HighlightCard(Card card)
   {
      card.transform.LeanScale(new Vector2(1.2f, 1.2f), 1f).setEase(LeanTweenType.easeOutBounce);
      card.GetComponent<CardControls>().enabled = false;

   }

   public void HighlightCardOff(Card card)
   {
      card.transform.LeanScale(Vector2.one, 1f);
      card.GetComponent<CardControls>().enabled = true;
   }

   public void ResetField()
   {
      foreach (var card in CardsOnPlayerField[0].Keys)
         SendToGraveyard(card, Player.PlayerOne);

      foreach (var card in CardsOnPlayerField[1].Keys)
         SendToGraveyard(card, Player.PlayerTwo);

      foreach (var weather in WeathersByPlayer)
         SendToGraveyard(weather.Key, weather.Value);

      CardsOnPlayerField[0].Clear();
      CardsOnPlayerField[1].Clear();
      WeathersByPlayer.Clear();
   }
}



