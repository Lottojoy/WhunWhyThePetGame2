using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("ตั้งค่า Map")]
    public List<MapSlot> allSlots = new List<MapSlot>();
    public LayerMask slotLayer;

    [Header("UI & Indicator")]
    public TMP_Text infoText;
    public GameObject arrowIndicatorPrefab;

    private MapSlot _currentHoveredSlot;
    private List<GameObject> _activeArrows = new List<GameObject>();

    private bool _isMoving = false;
    private GameObject _movingModel;
    private StationData _movingData;
    private System.Action _onMoveCompleteCallback;
    private bool _waitForMouseRelease = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetInfoText("Upgrade Clinic");
    }

    public void SetInfoText(string msg)
    {
        if (infoText != null)
        {
            infoText.text = msg;
        }
    }

    public void AutoPlaceStation(StationData data)
    {
        MapSlot emptySlot = allSlots.Find(s => !s.isOccupied);
        if (emptySlot != null)
        {
            GameObject newModel = Instantiate(data.modelPrefab);
            emptySlot.SetStation(data, newModel);
            SetInfoText("Upgrade Clinic");
        }
    }

    // 1. สำหรับโหมดย้ายของเดิม
    public void StartMoveMode(StationData data, System.Action onMoveComplete = null)
    {
        MapSlot currentSlot = allSlots.Find(s => s.isOccupied && s.currentStationID == data.stationID);

        if (currentSlot == null)
        {
            Debug.LogWarning($"[Map] ไม่พบตำแหน่งเดิมของ {data.stationName} ระบบจะเปลี่ยนเป็นการหยิบวางใหม่แทน");
            StartNewPlacementMode(data, onMoveComplete);
            return;
        }

        _movingData = data;
        _movingModel = currentSlot.placedModel;
        currentSlot.ClearSlot();

        SetupMovingModel(onMoveComplete);
    }

    // 2. [สำคัญ] สำหรับกรณี "ซื้อใหม่" แล้วหยิบมาวางเลยทันที
    public void StartNewPlacementMode(StationData data, System.Action onMoveComplete = null)
    {
        _movingData = data;

        if (data.modelPrefab != null)
        {
            _movingModel = Instantiate(data.modelPrefab);
        }
        else
        {
            Debug.LogError($"[Map] {data.stationName} ไม่มี Model Prefab!");
            return;
        }

        SetupMovingModel(onMoveComplete);
    }

    private void SetupMovingModel(System.Action onMoveComplete)
    {
        _isMoving = true;
        _waitForMouseRelease = true;
        _onMoveCompleteCallback = onMoveComplete;
        _currentHoveredSlot = null; // รีเซ็ตค่าช่องที่เคยชี้

        if (_movingModel == null) return;

        // ปิด Collider ของโมเดลที่กำลังถือ เพื่อไม่ให้บัง Raycast
        Collider[] colliders = _movingModel.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            if (col != null) col.enabled = false;
        }

        // ปรับ Layer
        int previewLayer = LayerMask.NameToLayer("ModelPreview");
        if (previewLayer != -1)
        {
            SetLayerRecursively(_movingModel.transform, previewLayer);
        }

        ClearAllArrows();

        // เสกลูกศรบนช่องว่างทั้งหมด
        if (arrowIndicatorPrefab != null)
        {
            foreach (MapSlot slot in allSlots)
            {
                if (slot != null && !slot.isOccupied)
                {
                    GameObject arrow = Instantiate(arrowIndicatorPrefab, slot.transform.position + Vector3.up * 2f, Quaternion.identity);
                    _activeArrows.Add(arrow);
                }
            }
        }

        SetInfoText("Select Placement Location!!!");
    }

    private void SetLayerRecursively(Transform trans, int newLayer)
    {
        if (trans == null) return;
        trans.gameObject.layer = newLayer;
        foreach (Transform child in trans)
        {
            if (child != null) SetLayerRecursively(child, newLayer);
        }
    }

    private void ClearAllArrows()
    {
        foreach (GameObject arrow in _activeArrows)
        {
            if (arrow != null) Destroy(arrow);
        }
        _activeArrows.Clear();
    }

    private void Update()
    {
        if (_isMoving && _movingModel != null)
        {
            // ระบบหมุนโมเดล (คลิกขวา หรือ ปุ่ม R)
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R))
            {
                _movingModel.transform.Rotate(0f, 90f, 0f);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, slotLayer))
            {
                MapSlot targetSlot = hit.collider.GetComponent<MapSlot>();
                if (targetSlot != null)
                {
                    if (!targetSlot.isOccupied)
                        _movingModel.transform.position = targetSlot.transform.position;
                    else
                        _movingModel.transform.position = targetSlot.transform.position + Vector3.up * 2f;

                    // --- ระบบซ่อนกล่องไม้เฉพาะช่องที่เมาส์ชี้ ---
                    if (targetSlot != _currentHoveredSlot)
                    {
                        if (_currentHoveredSlot != null && !_currentHoveredSlot.isOccupied)
                        {
                            _currentHoveredSlot.ShowEmptyModel();
                        }

                        _currentHoveredSlot = targetSlot;

                        if (_currentHoveredSlot != null && !_currentHoveredSlot.isOccupied)
                        {
                            _currentHoveredSlot.HideEmptyModel();
                        }
                    }

                    if (!targetSlot.isOccupied)
                        SetInfoText("Click Right/R to Rotate\nClick Left to Confirm");
                    else
                        SetInfoText("Select Placement Location!!!");

                    if (_waitForMouseRelease)
                    {
                        if (Input.GetMouseButtonUp(0)) _waitForMouseRelease = false;
                        return;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!targetSlot.isOccupied)
                        {
                            Collider[] colliders = _movingModel.GetComponentsInChildren<Collider>();
                            foreach (var col in colliders)
                            {
                                if (col != null) col.enabled = true;
                            }

                            targetSlot.SetStation(_movingData, _movingModel);
                            _isMoving = false;
                            _movingModel = null;
                            _movingData = null;
                            _currentHoveredSlot = null;

                            ClearAllArrows();
                            SetInfoText("Upgrade Clinic");

                            _onMoveCompleteCallback?.Invoke();
                            _onMoveCompleteCallback = null;
                        }
                    }
                }
            }
            else
            {
                // 1. คืนค่ากล่องไม้เปล่าให้ช่องล่าสุด
                if (_currentHoveredSlot != null)
                {
                    if (!_currentHoveredSlot.isOccupied) _currentHoveredSlot.ShowEmptyModel();
                    _currentHoveredSlot = null;
                }
                SetInfoText("Select Placement Location!!!");

                // 2. --- [เพิ่มโค้ดส่วนนี้] สั่งให้โมเดลลอยตามเมาส์แม้อยู่นอกช่อง ---
                // สร้างพื้นปูนจำลองล่องหนที่ระดับความสูง Y = 0
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

                // ถ้ายิงเลเซอร์จากเมาส์ไปกระทบพื้นจำลอง
                if (groundPlane.Raycast(ray, out float distance))
                {
                    Vector3 hitPoint = ray.GetPoint(distance);
                    // ให้โมเดลขยับตามเมาส์ และลอยสูงขึ้นมา 2 หน่วย (จะได้ไม่มุดดิน)
                    _movingModel.transform.position = new Vector3(hitPoint.x, 2f, hitPoint.z);
                }
            }
        }
    }
}