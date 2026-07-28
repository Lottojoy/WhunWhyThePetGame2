using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHoverEffect : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float transitionTime = 0.15f;

    private Dictionary<Transform, Coroutine> animations = new Dictionary<Transform, Coroutine>();

    public void OnHoverEnterEffect(GameObject obj)
    {
        AnimateScale(obj.transform, new Vector3(3.5f, 3.5f, 3.5f) * scaleMultiplier);
    }

    public void OnHoverExitEffect(GameObject obj)
    {
        AnimateScale(obj.transform, new Vector3(3.5f, 3.5f, 3.5f));
    }

    private void AnimateScale(Transform target, Vector3 targetScale)
    {
        if (animations.TryGetValue(target, out Coroutine running))
        {
            StopCoroutine(running);
        }

        animations[target] = StartCoroutine(ScaleCoroutine(target, targetScale));
    }

    private IEnumerator ScaleCoroutine(Transform target, Vector3 targetScale)
    {
        Vector3 startScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionTime;

            // Smooth easing
            t = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        target.localScale = targetScale;
        animations.Remove(target);
    }
}