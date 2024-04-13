using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HighlightEffect : MonoBehaviour
{
    [SerializeField] CanvasGroup HighlightImage;
    [SerializeField] float flickerSpeed;
    [SerializeField] float minAlpha;
    [SerializeField] float maxAlpha;

    void OnEnable()
    {
        minAlpha = 0;
        maxAlpha = 0.7f;
        flickerSpeed = 0.6f;
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        float startAlpha = minAlpha;
        float endAlpha = maxAlpha;
        while (true)
        {
            // Flicker effect
            float elapsedTime = 0f;
            while (elapsedTime < flickerSpeed)
            {
                elapsedTime += Time.deltaTime;
                HighlightImage.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / flickerSpeed);
                yield return null;
            }

            // Swap start and end alpha values for the next cycle
            float temp = startAlpha;
            startAlpha = endAlpha;
            endAlpha = temp;
        }
    }
}
