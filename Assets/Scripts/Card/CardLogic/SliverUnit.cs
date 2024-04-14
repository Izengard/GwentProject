using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;

public class SilverUnit : Unit, IPointerClickHandler
{
    [SerializeField] bool WeatherIsActive = false;
    [SerializeField] bool BuffIsActive = false;

    public new int Power
    {
        get
        {
            if (WeatherIsActive) return 1;
            else if (BuffIsActive) return 2 * power;
            else return power;
        }
        set => power = value;
    }

    public void SetWeather() => WeatherIsActive = true;
    public void ResetWeather() => WeatherIsActive = false;
    public void SetBuff() => BuffIsActive = true;
    public void ResetBuff() => BuffIsActive = false;
    public void ReturnToHand()
    {
        cardIsInHand = true;
        ResetWeather();
        ResetBuff();
        Power = UnitCardInfo.Power;
    }
    public new void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (GameManager.Instance.CurrentTurnPhase == TurnPhase.SelectCard)
            CardManager.Instance.HandleDecoyTarget(this);
    }

    void Update()
    {
        PowerText.text = Power.ToString();
        if (WeatherIsActive) PowerText.color = Color.red;
        else if (BuffIsActive) PowerText.color = Color.green;
        else PowerText.color = Color.black;
    }


}
