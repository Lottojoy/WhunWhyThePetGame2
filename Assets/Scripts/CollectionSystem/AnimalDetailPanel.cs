using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// หน้า Popup รายละเอียดสัตว์ (เปิดเมื่อกดที่ช่องสัตว์ในหน้า Collection)
/// ประกอบด้วย 3 ส่วนตามสเปก: โมเดล 3D / ชื่อ+คำอธิบาย / ปุ่มย้อนกลับ
///
/// ตั้งค่า 3D Preview ใน Editor:
/// 1. สร้าง Layer ใหม่ชื่อ "ModelPreview"
/// 2. สร้างกล้องแยกต่างหาก (Preview Camera) ที่ Culling Mask = เฉพาะ Layer "ModelPreview"
///    วาง Render Target เป็น RenderTexture ที่สร้างใหม่
/// 3. เอา RenderTexture นั้นไปใส่ใน RawImage บน UI (จุดที่จะโชว์โมเดล)
/// 4. modelSpawnPoint คือตำแหน่ง Transform ที่อยู่ในระยะที่ Preview Camera มองเห็น
///    และให้ modelPrefab ทุกตัวอยู่บน Layer "ModelPreview" เช่นกัน (รวมลูกๆ ของมันด้วย)
/// </summary>
public class AnimalDetailPanel : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject panelRoot;

    [Header("Popup Animation")]
    [Tooltip("ระยะเวลา animation ตอนเปิด/ปิด (วินาที)")]
    public float animationDuration = 0.25f;
    [Tooltip("เส้นโค้งควบคุมความเร็วของ animation ลองปรับ keyframe ให้เกิน 1 นิดๆ ตรงกลางถ้าอยากได้เอฟเฟกต์เด้ง (bounce)")]
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private RectTransform _panelRect;
    private Coroutine _popupCoroutine;
    private CanvasGroup _canvasGroup;

    [Header("3D Preview")]
    [Tooltip("จุด spawn โมเดล 3D อยู่ใน Layer ModelPreview มองผ่านกล้อง preview เท่านั้น")]
    public Transform modelSpawnPoint;
    [Tooltip("ความเร็วในการหมุนโมเดลอัตโนมัติ")]
    public float rotateSpeed = 20f;

    [Header("Silhouette (ตอนยังไม่ปลดล็อค)")]
    [Tooltip("Material สีดำล้วน (Unlit/Color ตั้งเป็นสีดำ) ใช้ทาโมเดลตอนยังไม่ปลดล็อค")]
    public Material silhouetteMaterial;

    [Header("Info")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    [Header("Locked / Unlocked Text Groups")]
    [Tooltip("กลุ่มข้อความที่โชว์ตอนยังไม่ปลดล็อค เช่น ข้อความ 'คุณยังไม่ปลดล็อคสัตว์ตัวนี้' (โมเดล 3D จะขึ้นเหมือนกันทั้งสองกรณี)")]
    public GameObject lockedGroup;
    [Tooltip("กลุ่มข้อความที่โชว์ตอนปลดล็อคแล้ว (ชื่อ + คำอธิบาย)")]
    public GameObject unlockedGroup;

    private GameObject _currentModelInstance;

    private void Awake()
    {
        _panelRect = panelRoot.GetComponent<RectTransform>();
        if (_panelRect == null)
        {
            Debug.LogWarning($"[AnimalDetailPanel] '{panelRoot.name}' ไม่มี RectTransform (ไม่ได้อยู่ใต้ Canvas หรือสร้างแบบ Create Empty) " +
                              "จะข้าม popup animation ไปเลย ให้ตรวจสอบว่า panelRoot อยู่ใต้ Canvas และเป็น UI element");
        }

        // ใช้ CanvasGroup บล็อคการกดปุ่มระหว่าง animation กำลังเล่นอยู่
        // (กันปัญหากดไม่ติด เพราะปุ่มยังไม่ขยายเต็มขนาดตอน animation ทำงาน)
        _canvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = panelRoot.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        // บังคับปิดตอนเริ่มเกม กันลืมปิด checkbox active ใน Editor
        // (ถ้าลืมปิดไว้ RawImage จะโชว์เป็นสีขาวเพราะ RenderTexture ยังไม่ได้ render อะไรเข้าไป)
        panelRoot.SetActive(false);
    }

    /// <summary>เรียกจาก CollectionManager ตอนกดช่องสัตว์</summary>
    public void Open(AnimalData data)
    {
        if (data == null) return;

        panelRoot.SetActive(true);

        // โมเดล 3D ขึ้นทั้งสองกรณี ต่างกันแค่สี (ปกติ / ดำสนิท)
        SpawnModel(data.modelPrefab, useSilhouette: !data.isUnlocked);

        if (!data.isUnlocked)
        {
            lockedGroup.SetActive(true);
            unlockedGroup.SetActive(false);
        }
        else
        {
            lockedGroup.SetActive(false);
            unlockedGroup.SetActive(true);

            nameText.text = data.animalName;
            descriptionText.text = data.description;
        }

        PlayPopupAnimation(0f, 1f, null);
    }

    private void SpawnModel(GameObject prefab, bool useSilhouette)
    {
        ClearModel();

        if (prefab == null || modelSpawnPoint == null) return;

        _currentModelInstance = Instantiate(prefab, modelSpawnPoint.position, modelSpawnPoint.rotation, modelSpawnPoint);

        if (useSilhouette)
        {
            ApplySilhouette(_currentModelInstance);
        }

        // ให้โมเดลหมุนดูรอบตัวเองอัตโนมัติ
        ModelAutoRotate rotator = _currentModelInstance.AddComponent<ModelAutoRotate>();
        rotator.rotateSpeed = rotateSpeed;
    }

    /// <summary>เปลี่ยนทุก material ในโมเดล (รวมลูกทุกชั้น) ให้เป็น silhouetteMaterial สีดำ</summary>
    private void ApplySilhouette(GameObject instance)
    {
        if (silhouetteMaterial == null)
        {
            Debug.LogWarning("[AnimalDetailPanel] ยังไม่ได้ใส่ silhouetteMaterial ใน Inspector");
            return;
        }

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            Material[] blackMats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < blackMats.Length; i++)
            {
                blackMats[i] = silhouetteMaterial;
            }
            rend.materials = blackMats;
        }
    }

    private void ClearModel()
    {
        if (_currentModelInstance != null)
        {
            // ซ่อนทันที เพราะ Destroy() จะมีผลจริงตอนจบเฟรม ถ้าไม่ซ่อนก่อน
            // ตอนสลับโมเดลเร็วๆ จะเห็นโมเดลเก่ากับใหม่ render ซ้อนกันชั่วขณะ
            _currentModelInstance.SetActive(false);
            Destroy(_currentModelInstance);
            _currentModelInstance = null;
        }
    }

    /// <summary>เรียกจากปุ่ม Back ของ UI นี้ (OnClick ใน Inspector)</summary>
    public void Close()
    {
        PlayPopupAnimation(1f, 0f, () =>
        {
            ClearModel();
            panelRoot.SetActive(false);
        });
    }

    /// <summary>เล่น animation ขยาย/หด scale ของ panelRoot จาก from ไป to แล้วเรียก onComplete ตอนจบ</summary>
    private void PlayPopupAnimation(float from, float to, System.Action onComplete)
    {
        if (_popupCoroutine != null)
        {
            StopCoroutine(_popupCoroutine);
        }
        _popupCoroutine = StartCoroutine(AnimateScaleRoutine(from, to, onComplete));
    }

    private IEnumerator AnimateScaleRoutine(float from, float to, System.Action onComplete)
    {
        // บล็อคการกดปุ่มไว้ก่อนระหว่าง animation ทำงาน
        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        if (_panelRect == null)
        {
            // ไม่มี RectTransform ให้ทำ ก็ข้าม animation ไปเลย แต่ยัง callback ต่อได้ปกติ
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
            onComplete?.Invoke();
            _popupCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        _panelRect.localScale = Vector3.one * from;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime; // ใช้ unscaledDeltaTime กันปัญหาถ้าเกมมี pause (Time.timeScale = 0)
            float progress = Mathf.Clamp01(elapsed / animationDuration);
            float curveT = animationCurve.Evaluate(progress);
            float scale = Mathf.LerpUnclamped(from, to, curveT);
            _panelRect.localScale = Vector3.one * scale;
            yield return null;
        }

        _panelRect.localScale = Vector3.one * to;

        // ปลดบล็อคหลัง animation จบ (เฉพาะตอนเปิด ไม่ต้องปลดตอนปิดเพราะ panel จะถูกซ่อนอยู่แล้ว)
        if (_canvasGroup != null && to > 0f)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        onComplete?.Invoke();
        _popupCoroutine = null;
    }
}