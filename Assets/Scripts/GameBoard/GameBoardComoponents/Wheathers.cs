using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LeanTween;

public class WeathersRow : MonoBehaviour
{
    [SerializeField] GameObject[] weathersSprites;
    [SerializeField] CanvasGroup clearingEffect;
    public GameObject Blizzard => weathersSprites[0];
    public GameObject Fog => weathersSprites[1];
    public GameObject Rain => weathersSprites[2];


    public void ClearingAnimation()
    {
        clearingEffect.gameObject.SetActive(true);
        StartCoroutine(BlinkAndBanish(clearingEffect));
    }

    public static IEnumerator BlinkAndBanish(CanvasGroup image)
    {
        float startAlpha = 1;
        float endAlpha = 0;
        float flickerSpeed = 1f;

        for (int i = 0; i < 3; i++)
        {
            float elapsedTime = 0f;
            while (elapsedTime < flickerSpeed)
            {
                elapsedTime += Time.deltaTime;
                image.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / flickerSpeed);
                yield return null;
            }

            // Swap start and end alpha values for the next cycle
            float temp = startAlpha;
            startAlpha = endAlpha;
            endAlpha = temp;
        }
    }
}