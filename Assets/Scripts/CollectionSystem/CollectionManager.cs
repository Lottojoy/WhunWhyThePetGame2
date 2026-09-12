using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    [Header("=== Panels ===")]
    [SerializeField] private GameObject collectionRootPanel; // ===Collection_UICanvas===
    [SerializeField] private CanvasGroup rootCanvasGroup;     // Component CanvasGroup บน Canvas หลัก
    [SerializeField] private GameObject petPanel;
    [SerializeField] private GameObject stationPanel;

    [Header("=== Transition Settings ===")]
    [Tooltip("ระยะเวลาอนิเมชันตอนเปิด/ปิด (วินาที)")]
    [SerializeField] private float transitionDuration = 0.25f;
    [Tooltip("เส้นโค้งควบคุมความเร็ว สามารถปรับดัดโค้งให้มีเอฟเฟกต์เด้ง (Bounce) ได้")]
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("=== Key Binding Setup ===")]
    [SerializeField] private KeyCode toggleKey = KeyCode.C;

    [Header("=== Category Buttons ===")]
    [SerializeField] private Button petTabBtn;
    [SerializeField] private Button stationTabBtn;
    [SerializeField] private Button backBtn;

    [Header("=== Grid Content Setup ===")]
    [SerializeField] private Transform petGridContent;
    [SerializeField] private Transform stationGridContent;
    [SerializeField] private GameObject slotPrefab;

    [Header("=== References ===")]
    [SerializeField] private CollectionDetailPanel detailPanel;

    [Header("=== Data List ===")]
    public List<AnimalData> animalList = new List<AnimalData>();
    public List<StationData> stationList = new List<StationData>();

    private GameObject currentPanel;
    private RectTransform _rootRectTransform;
    private Coroutine _transitionCoroutine;
    private bool _isOpen = false;

    private void Awake()
    {
        if (petPanel != null) currentPanel = petPanel;

        if (collectionRootPanel != null)
        {
            _rootRectTransform = collectionRootPanel.GetComponent<RectTransform>();
            if (rootCanvasGroup == null)
            {
                rootCanvasGroup = collectionRootPanel.GetComponent<CanvasGroup>();
                if (rootCanvasGroup == null)
                {
                    rootCanvasGroup = collectionRootPanel.AddComponent<CanvasGroup>();
                }
            }
        }
    }

    private void Start()
    {
        if (petTabBtn != null)
        {
            petTabBtn.onClick.RemoveAllListeners();
            petTabBtn.onClick.AddListener(ShowPetTab);
        }

        if (stationTabBtn != null)
        {
            stationTabBtn.onClick.RemoveAllListeners();
            stationTabBtn.onClick.AddListener(ShowStationTab);
        }

        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(CloseCollection);
        }

        PopulatePetGrid();
        PopulateStationGrid();

        // ซ่อนหน้าต่างเริ่มต้นโดยไม่เล่นอนิเมชัน
        SetCanvasStateDirectly(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleCollection();
        }
    }

    /// <summary>สลับสถานะเปิด/ปิดพร้อม Transition</summary>
    public void ToggleCollection()
    {
        if (_isOpen)
        {
            CloseCollection();
        }
        else
        {
            OpenCollection();
        }
    }

    public void OpenCollection()
    {
        _isOpen = true;
        if (collectionRootPanel != null) collectionRootPanel.SetActive(true);
        ResetToDefaultPanel();

        PlayTransition(isOpen: true);
    }

    public void CloseCollection()
    {
        _isOpen = false;
        PlayTransition(isOpen: false, onComplete: () =>
        {
            if (collectionRootPanel != null) collectionRootPanel.SetActive(false);
        });
    }

    private void PlayTransition(bool isOpen, System.Action onComplete = null)
    {
        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        _transitionCoroutine = StartCoroutine(AnimateCanvasRoutine(isOpen, onComplete));
    }

    private IEnumerator AnimateCanvasRoutine(bool isOpen, System.Action onComplete)
    {
        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.interactable = false;
            rootCanvasGroup.blocksRaycasts = false;
        }

        float startAlpha = rootCanvasGroup != null ? rootCanvasGroup.alpha : (isOpen ? 0f : 1f);
        float targetAlpha = isOpen ? 1f : 0f;

        Vector3 startScale = isOpen ? Vector3.one * 0.85f : Vector3.one;
        Vector3 targetScale = isOpen ? Vector3.one : Vector3.one * 0.85f;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / transitionDuration);
            float curveT = transitionCurve.Evaluate(progress);

            if (rootCanvasGroup != null)
            {
                rootCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveT);
            }

            if (_rootRectTransform != null)
            {
                _rootRectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveT);
            }

            yield return null;
        }

        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.alpha = targetAlpha;
            rootCanvasGroup.interactable = isOpen;
            rootCanvasGroup.blocksRaycasts = isOpen;
        }

        if (_rootRectTransform != null)
        {
            _rootRectTransform.localScale = targetScale;
        }

        onComplete?.Invoke();
        _transitionCoroutine = null;
    }

    private void SetCanvasStateDirectly(bool isOpen)
    {
        _isOpen = isOpen;
        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.alpha = isOpen ? 1f : 0f;
            rootCanvasGroup.interactable = isOpen;
            rootCanvasGroup.blocksRaycasts = isOpen;
        }
        if (collectionRootPanel != null)
        {
            collectionRootPanel.SetActive(isOpen);
        }
    }

    public void ShowPetTab()
    {
        if (currentPanel == petPanel && petPanel != null && petPanel.activeSelf) return;

        if (currentPanel != null) currentPanel.SetActive(false);
        if (petPanel != null)
        {
            petPanel.SetActive(true);
            currentPanel = petPanel;
        }
        if (stationPanel != null) stationPanel.SetActive(false);
    }

    public void ShowStationTab()
    {
        if (currentPanel == stationPanel && stationPanel != null && stationPanel.activeSelf) return;

        if (currentPanel != null) currentPanel.SetActive(false);
        if (stationPanel != null)
        {
            stationPanel.SetActive(true);
            currentPanel = stationPanel;
        }
        if (petPanel != null) petPanel.SetActive(false);
    }

    public void ResetToDefaultPanel()
    {
        if (petPanel != null) petPanel.SetActive(true);
        if (stationPanel != null) stationPanel.SetActive(false);
        currentPanel = petPanel;
    }

    private void PopulatePetGrid()
    {
        if (petGridContent == null || slotPrefab == null) return;

        for (int i = petGridContent.childCount - 1; i >= 0; i--)
        {
            Destroy(petGridContent.GetChild(i).gameObject);
        }

        foreach (AnimalData data in animalList)
        {
            if (data == null) continue;
            GameObject slotObj = Instantiate(slotPrefab, petGridContent);
            GenericSlotUI slotUI = slotObj.GetComponent<GenericSlotUI>();
            if (slotUI != null) slotUI.Setup(data, OnAnimalSlotClicked);
        }
    }

    private void PopulateStationGrid()
    {
        if (stationGridContent == null || slotPrefab == null) return;

        for (int i = stationGridContent.childCount - 1; i >= 0; i--)
        {
            Destroy(stationGridContent.GetChild(i).gameObject);
        }

        foreach (StationData data in stationList)
        {
            if (data == null) continue;
            GameObject slotObj = Instantiate(slotPrefab, stationGridContent);
            GenericSlotUI slotUI = slotObj.GetComponent<GenericSlotUI>();
            if (slotUI != null) slotUI.Setup(data, OnStationSlotClicked);
        }
    }

    private void OnAnimalSlotClicked(AnimalData data)
    {
        if (detailPanel != null) detailPanel.Open(data);
    }

    private void OnStationSlotClicked(StationData data)
    {
        if (detailPanel != null) detailPanel.Open(data);
    }
}