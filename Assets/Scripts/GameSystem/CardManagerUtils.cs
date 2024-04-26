using static LeanTween;
using UnityEngine;


public partial class CardManager : MonoBehaviour
{
   void PlayCardAudio(Card card) => card.GetComponent<AudioSource>().Play();

   public void ActivateEffect(Unit unit) => effectManager.ActivateUnitEffect(unit);

   public void ActivateLeaderEffect(Leader leader) => effectManager.ActivateLeaderEffect(leader);

   void AskForPlayerInput(Card pendingCard)
   {
      this.pendingCard = pendingCard;
      if (pendingCard is SpecialCard special && special.SpecialCardInfo.SpecialType == SpecialType.Decoy)
         HighlightSilverUnits(true);
      else
      {
         HighlightCard(pendingCard);
         currentField.HighlightRows(pendingCard);
      }
      CancelButton.gameObject.SetActive(true);
   }

   // Highlight the pending card by increasing its local scale, disable its controls to 
   // avoid rescaling on pointer exit
   void HighlightCard(Card card)
   {
      card.transform.LeanScale(new Vector2(1.2f, 1.2f), 1f).setEase(LeanTweenType.easeOutBounce);
      card.GetComponent<CardScaling>().enabled = false;

   }

   public void HighlightCardOff(Card card)
   {
      card.transform.LeanScale(Vector2.one, .7f);
      card.GetComponent<CardScaling>().enabled = true;
   }

   void HighlightSilverUnits(bool activate)
   {
      foreach (var card in SummonedCardsByRow.Keys)
      {
         if (card is SilverUnit silver && card.Owner == currentPlayer)
            if (activate) HighlightCard(silver);
            else HighlightCardOff(silver);
      }
   }

   public void CancelCardSelection()
   {
      CancelButton.gameObject.SetActive(false);
      HighlightCardOff(pendingCard);
      pendingCard.SetBackToHand();
      pendingCard = null;
      HighlightSilverUnits(false);
      currentField.HighlightRowsOff();
      GameManager.Instance.UpdateTurnPhase(TurnPhase.Play);
   }


   bool AnySilverUnit()
   {
      foreach (var card in SummonedCardsByRow.Keys)
         if (card is SilverUnit unit && card.Owner == currentPlayer) return true;

      return false;
   }

   static void CheckRowPowerMods(Unit unit, Row row)
   {
      if (unit is not SilverUnit silverUnit) return;

      if (row.WeatherIsActive)
         silverUnit.SetWeather();
      if (row.BuffIsActive)
         silverUnit.SetBuff();
   }

   public void MoveCardTo(Card card, Transform newPosition)
   {
      if (newPosition is null) return;
      card.transform.SetParent(gameBoard.transform);
      LeanTween.move(card.gameObject, newPosition.position, cardMoveDuration)
               .setOnComplete(PutInside);
      return;

      void PutInside()
      {
         card.transform.position = newPosition.transform.position;
         card.transform.SetParent(newPosition.transform);
      }
   }

   public void SendToGraveyard(Card card)
   {
      var graveyardTransform = graveyards[(int)card.Owner];
      MoveCardTo(card, graveyardTransform);
   }

   public void DestroyUnit(Unit unit)
   {
      var row = SummonedCardsByRow[unit];
      row.RemoveUnit(unit);
      SummonedCardsByRow.Remove(unit);
      CardManager.Instance.SendToGraveyard(unit);
   }

   public void ResetField()
   {
      foreach (var card in SummonedCardsByRow.Keys)
         SendToGraveyard(card);

      foreach (var weather in ActiveWeathers)
         SendToGraveyard(weather);

      SummonedCardsByRow.Clear();
      ActiveWeathers.Clear();
   }
}


