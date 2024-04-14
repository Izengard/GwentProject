using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpecialCard : Card
{
    [SerializeField] Image Artwork;
    public Image Type;
    public SpecialCardInfo cardInfo;

    public override CardInfo CardInfo => cardInfo;

    public override void SetCardInfo(CardInfo cardInfo)
    {
        this.cardInfo = cardInfo as SpecialCardInfo;
    }

    void Start()
    {
        gameObject.name = cardInfo.name;
        Artwork.sprite = cardInfo.Artwork;
        Type.sprite = cardInfo.Type;
    }
}
