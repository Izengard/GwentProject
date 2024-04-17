using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public abstract class Unit : Card
{
    [SerializeField] Image Artwork;
    public UnitCardInfo UnitCardInfo;
    public GameObject[] AttackTypeIcons;
    public TextMeshProUGUI PowerText;
    protected int power;
    public int Power => power;
    public override CardInfo CardInfo { get => UnitCardInfo; }
    public override void SetCardInfo(CardInfo unitCardInfo)
    {
        this.UnitCardInfo = unitCardInfo as UnitCardInfo;
    }

    void Start()
    {
        gameObject.name = UnitCardInfo.name;
        Artwork.sprite = UnitCardInfo.Artwork;
        power = UnitCardInfo.Power;
        PowerText.text = power.ToString();
        AttackTypeIcons[0].SetActive((UnitCardInfo.AttackTypes.Contains(AttackType.Melee)));
        AttackTypeIcons[1].SetActive((UnitCardInfo.AttackTypes.Contains(AttackType.Ranged)));
        AttackTypeIcons[2].SetActive((UnitCardInfo.AttackTypes.Contains(AttackType.Siege)));
    }
}
