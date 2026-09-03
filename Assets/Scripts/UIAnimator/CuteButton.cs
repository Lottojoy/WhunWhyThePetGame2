using UnityEngine;
using UnityEngine.EventSystems;

public class CuteButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public float hoverScale = 1.05f;
    public float clickScale = 0.92f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();

        StartCoroutine(
            ScaleTo(originalScale * hoverScale, 0.1f)
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();

        StartCoroutine(
            ScaleTo(originalScale, 0.1f)
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();

        StartCoroutine(
            ScaleTo(originalScale * clickScale, 0.06f)
        );
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();

        StartCoroutine(
            ScaleTo(originalScale * hoverScale, 0.12f)
        );
    }

    System.Collections.IEnumerator ScaleTo(
        Vector3 target,
        float duration
    )
    {
        Vector3 start = transform.localScale;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            transform.localScale =
                Vector3.Lerp(start, target, t);

            yield return null;
        }

        transform.localScale = target;
    }
}