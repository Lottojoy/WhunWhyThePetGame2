using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// หน้า Popup รายละเอียด Collection Generic (รองรับทั้ง AnimalData และ StationData)
/// </summary>
public class CollectionDetailPanel : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject panelRoot;
    public CanvasGroup panelCanvasGroup; // สำหรับ Fade Transition

    [Header("Popup Animation")]
    [Tooltip("ระยะเวลา animation ตอนเปิด/ปิด (วินาที)")]
    public float animationDuration = 0.25f;
    [Tooltip("เส้นโค้งควบคุมความเร็วของ animation")]
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private RectTransform _panelRect;
    private Coroutine _popupCoroutine;

    [Header("3D Preview (for Animal)")]
    [Tooltip("จุด spawn โมเดล 3D")]
    public Transform modelSpawnPoint;
    [Tooltip("ความเร็วในการหมุนโมเดลอัตโนมัติ")]
    public float rotateSpeed = 20f;
    [Tooltip("Material สีดำล้วน ทาโมเดลสัตว์ตอนยังไม่ปลดล็อค")]
    public Material silhouetteMaterial;

    [Header("Info Texts")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text unlockConditionText;

    [Header("Locked / Unlocked Text Groups")]
    [Tooltip("กลุ่ม UI ที่แสดงตอนยังไม่ปลดล็อค")]
    public GameObject lockedGroup;
    [Tooltip("กลุ่ม UI ที่แสดงตอนปลดล็อคแล้ว")]
    public GameObject unlockedGroup;

    private GameObject _currentModelInstance;

    private void Awake()
    {
        if (panelRoot != null)
        {
            _panelRect = panelRoot.GetComponent<RectTransform>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
                if (panelCanvasGroup == null)
                {
                    panelCanvasGroup = panelRoot.AddComponent<CanvasGroup>();
                }
            }
        }
    }

    private void Start()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    /// <summary>เรียกจาก CollectionManager ตอนกดช่องสัตว์ (AnimalData)</summary>
    public void Open(AnimalData data)
    {
        if (data == null) return;

        ResetPanelUI();
        panelRoot.SetActive(true);

        // เสกโมเดล 3D (ถ้ายังไม่ปลดล็อคจะกลายเป็นสีดำ)
        SpawnModel(data.modelPrefab, useSilhouette: !data.isUnlocked);

        if (!data.isUnlocked)
        {
            if (lockedGroup != null) lockedGroup.SetActive(true);
            if (unlockedGroup != null) unlockedGroup.SetActive(false);

            if (nameText != null) nameText.text = "???";
            if (descriptionText != null) descriptionText.text = "คุณยังไม่ปลดล็อคสัตว์ตัวนี้";
            if (unlockConditionText != null) unlockConditionText.text = data.unlockCondition;
        }
        else
        {
            if (lockedGroup != null) lockedGroup.SetActive(false);
            if (unlockedGroup != null) unlockedGroup.SetActive(true);

            if (nameText != null) nameText.text = data.animalName;
            if (descriptionText != null) descriptionText.text = data.description;
            if (unlockConditionText != null) unlockConditionText.text = "";
        }

        PlayPopupAnimation(0f, 1f, null);
    }

    /// <summary>เรียกจาก CollectionManager ตอนกดช่องสถานี (StationData)</summary>
    /// <summary>เรียกจาก CollectionManager ตอนกดช่องสถานี (StationData)</summary>
    public void Open(StationData data)
    {
        if (data == null) return;

        ResetPanelUI();
        if (panelRoot != null) panelRoot.SetActive(true);

        //  เปลี่ยนมาเรียก SpawnModel ของ Station (ถ้าไม่มี prefab ใส่ไว้ โมเดลจะไม่ขึ้นอัตโนมัติ)
        SpawnModel(data.modelPrefab, useSilhouette: !data.isUnlocked);

        if (!data.isUnlocked)
        {
            if (lockedGroup != null) lockedGroup.SetActive(true);
            if (unlockedGroup != null) unlockedGroup.SetActive(false);

            if (nameText != null) nameText.text = "???";
            if (descriptionText != null) descriptionText.text = "คุณยังไม่ปลดล็อคสถานีนี้";
            if (unlockConditionText != null) unlockConditionText.text = $"เงื่อนไข: {data.unlockCondition}";
        }
        else
        {
            if (lockedGroup != null) lockedGroup.SetActive(false);
            if (unlockedGroup != null) unlockedGroup.SetActive(true);

            if (nameText != null) nameText.text = data.stationName;
            if (descriptionText != null) descriptionText.text = data.description;
            if (unlockConditionText != null) unlockConditionText.text = "";
        }

        PlayPopupAnimation(0f, 1f, null);
    }

    private void ResetPanelUI()
    {
        if (lockedGroup != null) lockedGroup.SetActive(false);
        if (unlockedGroup != null) unlockedGroup.SetActive(false);

        if (nameText != null) nameText.text = "";
        if (descriptionText != null) descriptionText.text = "";
        if (unlockConditionText != null) unlockConditionText.text = "";
    }

    private void SpawnModel(GameObject prefab, bool useSilhouette)
    {
        ClearModel();
        if (prefab == null || modelSpawnPoint == null) return;
        _currentModelInstance = Instantiate(prefab, modelSpawnPoint.position, modelSpawnPoint.rotation, modelSpawnPoint);
        if (useSilhouette) ApplySilhouette(_currentModelInstance);

        ModelAutoRotate rotator = _currentModelInstance.AddComponent<ModelAutoRotate>();
        rotator.rotateSpeed = rotateSpeed;
    }

    private void ApplySilhouette(GameObject instance)
    {
        if (silhouetteMaterial == null) return;

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            Material[] blackMats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < blackMats.Length; i++) blackMats[i] = silhouetteMaterial;
            rend.materials = blackMats;
        }
    }

    private void ClearModel()
    {
        if (_currentModelInstance != null)
        {
            _currentModelInstance.SetActive(false);
            Destroy(_currentModelInstance);
            _currentModelInstance = null;
        }
    }

    public void Close()
    {
        PlayPopupAnimation(1f, 0f, () =>
        {
            ClearModel();
            if (panelRoot != null) panelRoot.SetActive(false);
        });
    }

    private void PlayPopupAnimation(float from, float to, System.Action onComplete)
    {
        if (_popupCoroutine != null) StopCoroutine(_popupCoroutine);
        _popupCoroutine = StartCoroutine(AnimateScaleRoutine(from, to, onComplete));
    }

    private IEnumerator AnimateScaleRoutine(float from, float to, System.Action onComplete)
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }

        if (_panelRect == null)
        {
            if (panelCanvasGroup != null) { panelCanvasGroup.interactable = true; panelCanvasGroup.blocksRaycasts = true; }
            onComplete?.Invoke();
            _popupCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        _panelRect.localScale = Vector3.one * from;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / animationDuration);
            float curveT = animationCurve.Evaluate(progress);
            float scale = Mathf.LerpUnclamped(from, to, curveT);
            _panelRect.localScale = Vector3.one * scale;
            yield return null;
        }

        _panelRect.localScale = Vector3.one * to;

        if (panelCanvasGroup != null && to > 0f)
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
        }

        onComplete?.Invoke();
        _popupCoroutine = null;
    }
}