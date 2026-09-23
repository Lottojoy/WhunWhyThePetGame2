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
    // เปลี่ยนมาใช้ List เพื่อเก็บลูกศรหลายๆ อัน
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
        // ใส่ Log ไว้เช็กด้วยว่าข้อความเปลี่ยนจริงไหม
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

    public void StartMoveMode(StationData data, System.Action onMoveComplete = null)
    {
        MapSlot currentSlot = allSlots.Find(s => s.isOccupied && s.currentStationID == data.stationID);

        if (currentSlot != null)
        {
            _movingData = data;
            _movingModel = currentSlot.placedModel;
            currentSlot.ClearSlot();
            _isMoving = true;
            _waitForMouseRelease = true;
            _onMoveCompleteCallback = onMoveComplete;

            // --- [เพิ่ม] 1. ปิด Collider ของโมเดลที่กำลังลาก เพื่อไม่ให้บังเมาส์ (Raycast) ---
            Collider[] colliders = _movingModel.GetComponentsInChildren<Collider>();
            foreach (var col in colliders) col.enabled = false;

            ClearAllArrows();

            if (arrowIndicatorPrefab != null)
            {
                foreach (MapSlot slot in allSlots)
                {
                    if (!slot.isOccupied)
                    {
                        GameObject arrow = Instantiate(arrowIndicatorPrefab, slot.transform.position + Vector3.up * 2f, Quaternion.identity);
                        _activeArrows.Add(arrow);
                    }
                }
            }

            SetInfoText("Select Placement Location!!!");
        }
    }

    // ฟังก์ชันช่วยลบลูกศรทั้งหมดทิ้งเมื่อวางเสร็จ
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
            // ระบบหมุนโมเดล
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

                    // --- ระบบจัดการกล่องไม้เปล่าเฉพาะช่องที่เมาส์ชี้ ---
                    if (targetSlot != _currentHoveredSlot)
                    {
                        // 1. ถ้าย้ายเมาส์หนีจากช่องเก่า และช่องนั้นว่าง -> เอาโคตรกล่องไม้กลับมาแสดง
                        if (_currentHoveredSlot != null && !_currentHoveredSlot.isOccupied)
                        {
                            _currentHoveredSlot.ShowEmptyModel();
                        }

                        // 2. อัปเดตช่องปัจจุบันที่เมาส์กำลังเล็ง
                        _currentHoveredSlot = targetSlot;

                        // 3. ถ้าช่องใหม่ที่เมาส์ชี้ว่างอยู่ -> สั่งซ่อนกล่องไม้เฉพาะช่องนี้
                        if (_currentHoveredSlot != null && !_currentHoveredSlot.isOccupied)
                        {
                            _currentHoveredSlot.HideEmptyModel();
                        }
                    }

                    // เช็กเปลี่ยนข้อความ
                    if (!targetSlot.isOccupied)
                        SetInfoText("Click Right/R to Rotate\nClick Left to Confirm");
                    else
                        SetInfoText("Select Placement Location!!!");

                    // ลอจิกป้องกันคลิกเบิ้ล
                    if (_waitForMouseRelease)
                    {
                        if (Input.GetMouseButtonUp(0)) _waitForMouseRelease = false;
                        return;
                    }

                    // ตอนคลิกซ้ายเพื่อวาง
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!targetSlot.isOccupied)
                        {
                            // เปิด Collider กลับมาทำงานตามปกติเมื่อวางเสร็จ
                            Collider[] colliders = _movingModel.GetComponentsInChildren<Collider>();
                            foreach (var col in colliders) col.enabled = true;

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
                if (_currentHoveredSlot != null)
                {
                    if (!_currentHoveredSlot.isOccupied) _currentHoveredSlot.ShowEmptyModel();
                    _currentHoveredSlot = null;
                }
                // [แก้ไขตรงนี้] เอาคำสั่งโชว์กล่องเปล่าคืนออก 
                // โมเดลจะเกาะอยู่ที่ช่องล่าสุดที่เมาส์ชี้ผ่าน และกล่องเปล่าจะยังคงซ่อนอยู่
                SetInfoText("Select Placement Location!!!");
            }
        }
    }
}