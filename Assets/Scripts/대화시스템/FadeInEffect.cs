using UnityEngine;
using System.Collections;

public class FadeInEffect : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float duration = 1.5f;

    void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = true;
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}
