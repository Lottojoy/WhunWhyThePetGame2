using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonSlotSave : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scale Settings")]
    public float scaleMultiplier = 1.05f;
    public float duration = 0.15f;
    public Ease easeType = Ease.OutQuad;
    
    public int slotId = 0;

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
    private ClinicSaveData slotMapData;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        // Detect left clicks
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("UI Panel clicked!");
            // OnPanelClicked();

            ClinicSaveData mapData = ClinicSaveManager.Instance.GetMap(slotId);

            if (mapData == null)
            {
                SlotMapManager.Instance.inputScreenPanel.SetActive(true);
                SlotMapManager.Instance.slotMapId = slotId;
            } else
            {
                
            }
            
            
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