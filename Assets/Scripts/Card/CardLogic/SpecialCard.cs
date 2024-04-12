using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpecialCard : Card
{
    [SerializeField] Image Artwork;
    public Image Type;
    public SpecialCardInfo card;

    public override CardInfo CardInfo => card;

    public override void SetCardInfo(CardInfo cardInfo)
    {
        this.card = cardInfo as SpecialCardInfo;
    }

    void Start()
    {
        Artwork.sprite = card.Artwork;
        Type.sprite = card.Type;
    }
}
