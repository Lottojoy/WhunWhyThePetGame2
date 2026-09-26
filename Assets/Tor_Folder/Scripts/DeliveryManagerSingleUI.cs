using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private Image animalIconImage;
    [SerializeField] private Transform actionIconContainer;
    [SerializeField] private GameObject actionIconPrefab;
    [SerializeField] private StationListSO stationListSO;
    [SerializeField] private Image timerFillImage; // <-- เพิ่มใหม่ ลากรูป bar สีเขียวมาใส่

    private RectTransform rectTransform;
    private AnimalOrder currentOrder;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetOrder(AnimalOrder order, bool isNewItem)
    {
        currentOrder = order;

        if (animalIconImage != null && order.animalData.orderIconSprite != null)
        {
            animalIconImage.sprite = order.animalData.orderIconSprite;
        }

        if (actionIconContainer != null)
        {
            foreach (Transform child in actionIconContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (StationType station in order.requiredStations)
            {
                Sprite icon = GetIconForStation(station);
                if (icon == null) continue;

                GameObject iconGO = Instantiate(actionIconPrefab, actionIconContainer);
                Image iconImage = iconGO.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = icon;
                }
            }
        }

        if (isNewItem)
        {
            StartCoroutine(SlideInAnimation());
        }
    }

    private void Update()
    {
        if (currentOrder == null || timerFillImage == null || currentOrder.patienceTime <= 0f) return;

        float elapsed = Time.time - currentOrder.spawnTime;
        float remainingRatio = 1f - (elapsed / currentOrder.patienceTime);
        timerFillImage.fillAmount = Mathf.Clamp01(remainingRatio);
    }

    private Sprite GetIconForStation(StationType type)
    {
        if (stationListSO == null) return null;

        foreach (StationData station in stationListSO.stationList)
        {
            if (station.stationType == type)
            {
                return station.colorSprite;
            }
        }
        return null;
    }

    private IEnumerator SlideInAnimation()
    {
        yield return null;

        Vector2 targetPosition = rectTransform.anchoredPosition;
        Vector2 startPosition = targetPosition + new Vector2(300f, 0f);
        rectTransform.anchoredPosition = startPosition;

        float duration = 0.35f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }
}