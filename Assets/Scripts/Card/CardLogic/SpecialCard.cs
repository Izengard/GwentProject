using UnityEngine;
using UnityEngine.UI;


public class SpecialCard : Card
{
    [SerializeField] Image Artwork;
    public Image Type;
    public SpecialCardInfo SpecialCardInfo;

    public override CardInfo CardInfo => SpecialCardInfo;

    public override void SetCardInfo(CardInfo cardInfo)
    {
        this.SpecialCardInfo = cardInfo as SpecialCardInfo;
    }

    void Start()
    {
        gameObject.name = SpecialCardInfo.name;
        Artwork.sprite = SpecialCardInfo.Artwork;
        Type.sprite = SpecialCardInfo.Type;
    }
}
