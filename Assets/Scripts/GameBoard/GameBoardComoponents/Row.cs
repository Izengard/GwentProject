using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Row : MonoBehaviour
{
    public Transform BuffTransform;
    public Transform RowUnitsTransform;
    public bool BuffIsActive { get; private set; }
    public bool WeatherIsActive { get; private set; }
    [field: SerializeField] public List<Unit> rowUnits { get; private set; } = new List<Unit>();
    [SerializeField] HighlightEffect highlightEffect;
    [SerializeField] TextMeshProUGUI PowerSubtotalScore;
    [SerializeField] GameObject WeatherEffect;
    [SerializeField] AttackType attackType;
    public AttackType AttackType => attackType;
    [SerializeField] int decoyCount = 0;


    public int PowerSubtotal
    {
        get
        {
            int powerSubtotal = 0;
            foreach (var card in rowUnits)
            {
                if (card is GoldenUnit golden) powerSubtotal += golden.Power;
                if (card is SilverUnit silver) powerSubtotal += silver.Power;
            }
            return powerSubtotal;
        }
    }

    public int UnitsCount => rowUnits.Count + decoyCount;


    public void AddUnit(Unit card) => this.rowUnits.Add(card);
    public void RemoveUnit(Unit card) => this.rowUnits.Remove(card);
    public void AddDecoy() => decoyCount++;
    public void ResetDecoys() => decoyCount = 0;
    public void SetWeather()
    {
        this.WeatherIsActive = true;
        foreach (var unit in rowUnits)
            if (unit is SilverUnit silver)
                silver.SetWeather();
    }
    public void ResetWeather()
    {
        this.WeatherIsActive = false;
        foreach (var unit in rowUnits)
            if (unit is SilverUnit silver)
                silver.ResetWeather();
    }
    public void ActivateBuff()
    {
        this.BuffIsActive = true;
        foreach (var unit in rowUnits)
            if (unit is SilverUnit silver)
                silver.SetBuff();
    }

    public void ResetBuff()
    {
        this.BuffIsActive = false;
        foreach (var unit in rowUnits)
            if (unit is SilverUnit silver)
                silver.ResetBuff();
    }

    public void DestroyAllSilverUnits()
    {
        foreach (var unit in rowUnits.ToArray())
            if (unit is SilverUnit silverUnit)
                CardManager.Instance.DestroyUnit(silverUnit);
    }
    public void ResetRow()
    {
        ResetWeather();
        ResetBuff();
        ResetDecoys();
        rowUnits.Clear();
    }

    public void HighlightOn() => this.highlightEffect.gameObject.SetActive(true);
    public void HighlightOff()
    {
        highlightEffect.StopAllCoroutines();
        this.highlightEffect.gameObject.SetActive(false);
    }
    public void OnClick()
    {
        CardManager.Instance.HandleRowSelection(this);
    }

    void Update()
    {
        PowerSubtotalScore.text = PowerSubtotal.ToString();

        // Weathers
        if (WeatherIsActive)
            PowerSubtotalScore.color = Color.red;
        else if (BuffIsActive) PowerSubtotalScore.color = Color.green;
        else
            PowerSubtotalScore.color = Color.black;

        WeatherEffect.SetActive(WeatherIsActive);
    }
}
