using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButtonSlotSave : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    public float scaleMultiplier = 1.05f;
    public float duration = 0.15f;
    public Ease easeType = Ease.OutQuad;

    [Header("Color Settings")]
    [SerializeField] private Image targetImage; // Drag 'BgSlot' Image here
    
    [Tooltip("Value between 0 (black) and 1 (original color). E.g., 0.8 reduces brightness by 20%.")]
    [Range(0f, 1f)]
    public float colorDropFactor = 0.8f; // Multiplier to darken normal color

    private Color normalColor;
    private Color hoverColor;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale * scaleMultiplier;

        // Auto-find target image fallback if not assigned
        if (targetImage == null)
        {
            Transform bgSlotTransform = transform.Find("Empty/Bg/BgSlot");
            if (bgSlotTransform != null)
            {
                targetImage = bgSlotTransform.GetComponent<Image>();
            }
        }

        if (targetImage != null)
        {
            normalColor = targetImage.color;
            
            // Multiply RGB channels by the factor while preserving original Alpha
            hoverColor = new Color(
                normalColor.r * colorDropFactor,
                normalColor.g * colorDropFactor,
                normalColor.b * colorDropFactor,
                normalColor.a
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOKill();
        rectTransform.DOScale(targetScale, duration).SetEase(easeType).SetUpdate(true);

        if (targetImage != null)
        {
            targetImage.DOKill();
            targetImage.DOColor(hoverColor, duration).SetEase(easeType).SetUpdate(true);
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOKill();
        rectTransform.DOScale(originalScale, duration).SetEase(easeType).SetUpdate(true);

        if (targetImage != null)
        {
            targetImage.DOKill();
            targetImage.DOColor(normalColor, duration).SetEase(easeType).SetUpdate(true);
        }
    }

    private void OnDisable()
    {
        rectTransform.DOKill();
        rectTransform.localScale = originalScale;

        if (targetImage != null)
        {
            targetImage.DOKill();
            targetImage.color = normalColor;
        }
    }
}