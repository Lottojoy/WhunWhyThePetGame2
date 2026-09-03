using UnityEngine;
using System.Collections;

public class CuteUIAnimator : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;

        originalScale = transform.localScale;
        if (originalScale == Vector3.zero)
            originalScale = Vector3.one;   // กันเคส template ถูกซ่อนด้วยสเกล 0

        originalPosition = transform.localPosition;
    }

    // =========================
    // POP IN
    // =========================
    public void PopIn(float delay = 0f)
    {
        StartCoroutine(PopInRoutine(delay));
    }

    IEnumerator PopInRoutine(float delay)
    {
        transform.localScale = Vector3.zero;   // ย้ายมาไว้ตรงนี้ ก่อน delay

        yield return new WaitForSeconds(delay);

        float time = 0f;
        float duration = 0.35f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float scale = EaseOutBack(t);
            transform.localScale = originalScale * scale;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    // =========================
    // BOUNCE
    // =========================
    public void Bounce()
    {
        StartCoroutine(BounceRoutine());
    }

    IEnumerator BounceRoutine()
    {
        Vector3 start = originalScale;

        float duration = 0.25f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;

            transform.localScale = start * scale;

            yield return null;
        }

        transform.localScale = start;
    }

    // =========================
    // SQUISH
    // =========================
    public void Squish()
    {
        StartCoroutine(SquishRoutine());
    }

    IEnumerator SquishRoutine()
    {
        Vector3 start = originalScale;

        // ยุบ
        transform.localScale = new Vector3(
            start.x * 1.08f,
            start.y * 0.88f,
            start.z
        );

        yield return new WaitForSeconds(0.08f);

        // ยืด
        transform.localScale = new Vector3(
            start.x * 0.95f,
            start.y * 1.08f,
            start.z
        );

        yield return new WaitForSeconds(0.08f);

        // กลับ
        transform.localScale = start;
    }

    // =========================
    // FLOAT
    // =========================
    public void StartFloating(
        float height = 5f,
        float speed = 1f
    )
    {
        StartCoroutine(FloatRoutine(height, speed));
    }

    IEnumerator FloatRoutine(float height, float speed)
    {
        Vector3 start = originalPosition;

        while (true)
        {
            float y = Mathf.Sin(Time.time * speed) * height;

            transform.localPosition =
                start + new Vector3(0, y, 0);

            yield return null;
        }
    }

    // =========================
    // EASE OUT BACK
    // =========================
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1 +
            c3 * Mathf.Pow(t - 1, 3) +
            c1 * Mathf.Pow(t - 1, 2);
    }
}