using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class InfoDisplay : MonoBehaviour
{
    public static InfoDisplay Instance { get; private set; }
    [SerializeField] Image cardImage;
    [SerializeField] Image Faction;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Description;
    [SerializeField] TextMeshProUGUI Effect;
    [SerializeField] Scrollbar scrollbar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void DisplayCardInfo(CardInfo info)
    {
        scrollbar.value = 1;
        cardImage.sprite = info.Artwork;
        Faction.sprite = info.FactionLogo;
        Name.text = info.Name;
        Description.text = info.Description;

        Effect.text =
        string.Concat(info.effect.ToString()
              .Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
    }
}
