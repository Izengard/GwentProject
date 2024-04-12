using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    [SerializeField] private SilverUnit Silver;
    [SerializeField] private GoldenUnit Golden;
    [SerializeField] private SpecialCard Special;

    public Card InstantiateCard(CardInfo cardInfo)
    {
        Card drawnCard = null;
        switch (cardInfo)
        {
            case SpecialCardInfo special:
                drawnCard = Instantiate(Special, this.transform);
                drawnCard.SetCardInfo(special);
                break;
            case UnitCardInfo unitCard when unitCard.UnitType == UnitType.Silver:
                drawnCard = Instantiate(Silver, this.transform);
                break;
            case UnitCardInfo unitCard when unitCard.UnitType == UnitType.Golden:
                drawnCard = Instantiate(Golden, this.transform);
                break;
            default:
                Debug.LogError("Unsupported card type.");
                break;
        }

        if (drawnCard != null)
            drawnCard.SetCardInfo(cardInfo);

        return drawnCard;
    }
}