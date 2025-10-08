using UnityEngine;
using System;
using System.Collections;

public class FadeInEffect : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float duration = 1.5f;

    public Action onFadeComplete; // 콜백 추가

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

        // 완료 시 콜백 실행
        onFadeComplete?.Invoke();
    }
}
