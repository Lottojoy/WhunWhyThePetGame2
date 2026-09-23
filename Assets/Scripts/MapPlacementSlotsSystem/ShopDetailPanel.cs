using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopDetailPanel : MonoBehaviour
{
    [Header("Panel Root & Animation")]
    public GameObject panelRoot;
    public CanvasGroup panelCanvasGroup;
    public float animationDuration = 0.25f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private RectTransform _panelRect;
    private Coroutine _popupCoroutine;

    [Header("3D Preview")]
    public Transform modelSpawnPoint;
    public float rotateSpeed = 20f;
    private GameObject _currentModelInstance;

    [Header("Info Texts")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    [Header("Performance Stats Texts")]
    public TMP_Text processTimeText;
    public TMP_Text satisfactionText;
    public TMP_Text serviceFeeText;

    [Header("Buttons")]
    public Button upgradeBtn;
    public TMP_Text upgradeBtnText; // Text ย่อยในปุ่ม Upgrade
    public Button moveBtn;
    public Button closeBtn;

    // เก็บ Callback ไว้รันตอนกดปุ่ม
    private System.Action _onUpgradeClicked;
    private System.Action _onMoveClicked;

    private void Awake()
    {
        if (panelRoot != null)
        {
            _panelRect = panelRoot.GetComponent<RectTransform>();
            if (panelCanvasGroup == null) panelCanvasGroup = panelRoot.AddComponent<CanvasGroup>();
        }

        // ผูกปุ่มล่วงหน้า
        closeBtn.onClick.AddListener(Close);
        upgradeBtn.onClick.AddListener(() => _onUpgradeClicked?.Invoke());
        moveBtn.onClick.AddListener(() => _onMoveClicked?.Invoke());
    }

    private void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    /// <summary>
    /// ฟังก์ชันเปิด Popup รับข้อมูลมาแสดงผล
    /// </summary>
    public void Open(StationData data, bool isOwned, int currentLevel, System.Action onUpgrade, System.Action onMove)
    {
        if (data == null) return;

        _onUpgradeClicked = onUpgrade;
        _onMoveClicked = onMove;

        panelRoot.SetActive(true);
        SpawnModel(data.modelPrefab);

        nameText.text = data.stationName;
        descriptionText.text = data.description;

        // ดึง Stats มาแสดง (ถ้าซื้อแล้วดึงเวลปัจจุบัน ถ้ายังไม่ซื้อดึงเวล 1)
        int displayLevel = isOwned ? currentLevel : 1;
        StationLevelStats stats = data.GetStatsByLevel(displayLevel);

        processTimeText.text = $"เวลา: {stats.processTime}s";
        satisfactionText.text = $"ความพอใจ: +{stats.satisfactionBonus}";
        serviceFeeText.text = $"ค่าบริการ: {stats.serviceFee}";

        // จัดการปุ่ม Move
        moveBtn.gameObject.SetActive(isOwned); // ถ้ายังไม่ซื้อ จะซ่อนปุ่มย้าย

        // จัดการปุ่ม Upgrade / Buy
        if (!isOwned)
        {
            upgradeBtn.interactable = true;
            upgradeBtnText.text = $"Buy: {stats.upgradeCost}";
        }
        else
        {
            if (currentLevel >= 4) // สมมติว่า Max Level คือ 4
            {
                upgradeBtn.interactable = false;
                upgradeBtnText.text = "Lv.MAX";
            }
            else
            {
                upgradeBtn.interactable = true;
                StationLevelStats nextStats = data.GetStatsByLevel(currentLevel + 1);
                upgradeBtnText.text = $"Upgrade (Lv.{currentLevel + 1}): {nextStats.upgradeCost}";
            }
        }

        PlayPopupAnimation(0f, 1f, null);
    }

    // --- ส่วนจัดการโมเดล 3D และ อนิเมชัน (เหมือนเดิม) ---
    private void SpawnModel(GameObject prefab)
    {
        ClearModel();
        if (prefab == null || modelSpawnPoint == null) return;
        _currentModelInstance = Instantiate(prefab, modelSpawnPoint.position, modelSpawnPoint.rotation, modelSpawnPoint);

        // เพิ่มหมุนอัตโนมัติ (ถ้ายังไม่มีสคริปต์นี้ในโปรเจกต์ อาจจะต้องสร้างเพิ่ม หรือลบออก)
         ModelAutoRotate rotator = _currentModelInstance.AddComponent<ModelAutoRotate>();
         rotator.rotateSpeed = rotateSpeed;
    }

    private void ClearModel()
    {
        if (_currentModelInstance != null)
        {
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
        if (panelCanvasGroup != null) panelCanvasGroup.blocksRaycasts = false;
        float elapsed = 0f;
        _panelRect.localScale = Vector3.one * from;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / animationDuration);
            float curveT = animationCurve.Evaluate(progress);
            _panelRect.localScale = Vector3.one * Mathf.LerpUnclamped(from, to, curveT);
            yield return null;
        }
        _panelRect.localScale = Vector3.one * to;
        if (panelCanvasGroup != null && to > 0f) panelCanvasGroup.blocksRaycasts = true;
        onComplete?.Invoke();
    }
}