using static LeanTween;
using System.Collections.Generic;
using UnityEngine;
using static GameBoard.Weather;
using System.Linq;

public class CardManager : MonoBehaviour
{
   // Singleton Pattern CardManager
   public static CardManager Instance { get; private set; }
   private GameBoard gameBoard;
   [SerializeField] private Transform[] graveyards;
   [SerializeField] private EffectManager effectManager;
   Player currentPlayer => GameManager.Instance.currentPlayer;
   Battlefield currentField => gameBoard.PlayerBattlefield[(int)currentPlayer];

   // Let's keep a reference of all the cards that has been summoned so we can quickly manage them
   public Dictionary<Card, Row>[] CardsOnPlayerField { get; private set; } = new Dictionary<Card, Row>[2];
   List<SpecialCard> Weathers = new();
   Card pendingCard;

   void Awake()
   {
      if (Instance == null) Instance = this;
      else if (Instance != this) Destroy(gameObject);
      CardsOnPlayerField[0] = new();
      CardsOnPlayerField[1] = new();
      gameBoard = GameBoard.Instance;
   }

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
      var attacks = unit.card.AttackTypes;
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
      Transform destination;
      switch (special.card.SpecialType)
      {
         case SpecialType.Blizzard:
            if (gameBoard.IsWeatherActive(Blizzard))
            {
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }
            Weathers.Add(special);
            destination = gameBoard.Weathers.Blizzard.transform;
            gameBoard.SetWeather(Blizzard);
            Debug.Log($" Blizzard Card Summoned");
            break;

         case SpecialType.Fog:
            if (gameBoard.IsWeatherActive(Fog))
            {
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }
            Weathers.Add(special);
            destination = gameBoard.Weathers.Fog.transform;
            gameBoard.SetWeather(Fog);
            Debug.Log($"Fog Card Summoned");
            break;

         case SpecialType.Rain:
            if (gameBoard.IsWeatherActive(Rain))
            {
               GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
               return;
            }
            Weathers.Add(special);
            destination = gameBoard.Weathers.Rain.transform;
            gameBoard.SetWeather(Rain);
            Debug.Log($"Rain Card Summoned");
            break;

         case SpecialType.Clearing:
            gameBoard.ResetWeather();
            destination = graveyards[(int)currentPlayer];
            foreach (var card in Weathers)
               SendToGraveyard(card);
            Debug.Log($"Clearing Card Played");

            break;

         case SpecialType.Buff:
            destination = null;
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
            destination = null;
            Debug.Log($"Decoy Card Selected");
            break;

         default:
            destination = null;
            Debug.Log($"No Valid Card Type detected");
            break;
      }
      if (destination is not null)
      {
         MoveCardTo(special, destination);
         delayedCall(1, () =>
         GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd));
      }

      bool AnySilverUnit()
      {
         foreach (var card in CardsOnPlayerField[(int)currentPlayer].Keys)
         {
            if (card is SilverUnit unit) return true;
         }
         return false;
      }
   }

   void HighlightAllSilverUnits(bool isOn)
   {
      foreach (var card in CardsOnPlayerField[(int)currentPlayer].Keys)
      {
         if (card is SilverUnit silver)
            if (isOn) HighlightCard(silver);
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
         GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd);
      }
      pendingCard = null;
   }

   public void HandleDecoyTarget(SilverUnit unit)
   {
      if(!CardsOnPlayerField[(int)currentPlayer].Keys.Contains(unit))
      {
         GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
         return;
      }

      unit.ResetBuff(); unit.ResetWeather();
      unit.ReturnToHand();
      HighlightCardOff(pendingCard);
      HighlightAllSilverUnits(false);

      var row = CardsOnPlayerField[(int)currentPlayer][unit];
      row.RemoveUnit(unit);
      CardsOnPlayerField[(int)currentPlayer].Remove(unit);
      CardsOnPlayerField[(int)currentPlayer].Add(pendingCard, row);

      MoveCardTo(pendingCard, unit.transform.parent);
      var hand = gameBoard.PlayerBoards[(int)currentPlayer].Hand.transform;
      MoveCardTo(unit, hand);
      delayedCall(1, () =>
      GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd));
   }

   public void CancelCardSelection()
   {
      pendingCard = null;
      HighlightAllSilverUnits(false);
      currentField.HighlightRowsOff();
      GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
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

   void SummonUnitCardInRow(Unit unit, Row row)
   {
      unit.GetComponent<AudioSource>().Play();
      row.AddUnit(unit);
      CardManager.Instance.CardsOnPlayerField[(int)currentPlayer].Add(unit, row);
      MoveCardTo(unit, row.RowUnits);
      CheckRowPowerMods(unit, row);
      // delayedCall(1f, () =>
      // effectManager.ActivateUnitEffect(unit));
      delayedCall(1f, () =>
      GameManager.Instance.UpdateTurnPhase(TurnPhase.TurnEnd));
   }

   public void MoveCardTo(Card card, Transform destination)
   {
      if (destination is null) return;

      card.transform.SetParent(gameBoard.transform);
      LeanTween.move(card.gameObject, destination.position, 1f).setOnComplete(PutInside);

      void PutInside()
      {
         card.transform.position = destination.transform.position;
         card.transform.SetParent(destination.transform);
      }
   }

   public void SendToGraveyard(Card card)
   {
      var graveyardTransform = graveyards[(int)currentPlayer];
      MoveCardTo(card, graveyardTransform);
   }
   
   // Highlight the pending card by increasing its local scale, disable its controls to 
   // avoid rescaling on pointer exit, then make it blink using LeanTween.alpha
   void HighlightCard(Card card)
   {
      card.transform.LeanScale(new Vector2(1.2f, 1.2f), 1f).setEase(LeanTweenType.easeOutBounce);
      card.GetComponent<CardControls>().enabled = false;
      LeanTween.alpha(card.gameObject, 0.5f, 1.5f).setOnComplete(() => LeanTween.alpha(card.gameObject, 1f, 1f));
   }
   public void HighlightCardOff(Card card)
   {
      card.transform.LeanScale(Vector2.one, 1f);
      card.GetComponent<CardControls>().enabled = true;
   }
   public void ResetField()
   {
      foreach (var card in CardsOnPlayerField[0].Keys)
         SendToGraveyard(card);

      foreach (var card in CardsOnPlayerField[1].Keys)
         SendToGraveyard(card);

      foreach (var card in Weathers)
         SendToGraveyard(card);

      CardsOnPlayerField[0].Clear();
      CardsOnPlayerField[1].Clear();
      Weathers.Clear();
   }
}



