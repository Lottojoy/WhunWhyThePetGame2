using System.Collections;
using UnityEngine;

public class CharacterSelectorManager : MonoBehaviour
{
    public Transform Characters;

    [SerializeField] private float spacing = -3f;      // Distance between characters
    [SerializeField] private float moveTime = 0.3f;

    private int currentIndex = 0;
    private Coroutine moveCoroutine;

    public void OnClickSelectAvatar()
    {
        Debug.Log(Characters.GetChild(currentIndex));
    }

    public void OnClickBack()
    {
        Debug.Log("Back");
    }

    public void OnClickRight()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            MoveToCurrent();
        }
    }

    public void OnClickLeft()
    {
        if (currentIndex < Characters.childCount - 1)
        {
            currentIndex++;
            MoveToCurrent();
        }
    }

    private void MoveToCurrent()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        Vector3 targetPos = new Vector3(-currentIndex * spacing, 5.5f, -6.5f);
        moveCoroutine = StartCoroutine(SmoothMove(targetPos));
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        Vector3 start = Characters.localPosition;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveTime);

            Characters.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        Characters.localPosition = target;
    }
}