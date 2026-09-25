using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("=== Grid Setup ===")]
    public Transform shopGridContent;
    public GameObject shopSlotPrefab;

    [Header("=== Data List ===")]
    public List<StationData> allStations = new List<StationData>();

    [Header("=== References ===")]
    public ShopDetailPanel detailPanel;

    [Header("=== Player Currency ===")]
    public int playerMoney = 1500;
    public TMP_Text moneyText;

    [Header("=== Slide Animation ===")]
    [Tooltip("ลากกรอบ UI ของหน้าต่าง Shop มาใส่ที่นี่ (เพื่อให้มันเลื่อนทั้งแผง)")]
    public RectTransform shopPanelRect;
    [Tooltip("ระยะที่ UI จะเลื่อนออกไปทางซ้าย (พิกเซล)")]
    public float slideDistance = 1500f;
    [Tooltip("ระยะเวลาที่ใช้เลื่อน (วินาที)")]
    public float slideDuration = 0.3f;

    // เก็บข้อมูล Station ที่ผู้เล่นมี (ID -> Level)
    private Dictionary<string, int> _playerOwnedStations = new Dictionary<string, int>();

    // ตัวแปรสำหรับทำอนิเมชันสไลด์
    private Vector2 _originalPosition;
    private Coroutine _slideCoroutine;

    private void Start()
    {
        // จำตำแหน่งเริ่มต้นของ UI Shop ไว้
        if (shopPanelRect != null)
        {
            _originalPosition = shopPanelRect.anchoredPosition;
        }

        UpdateMoneyUI();
        PopulateShop();
    }

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {playerMoney}";
        }
    }

    public void PopulateShop()
    {
        // ล้างของเก่าใน Grid ออกก่อน
        for (int i = shopGridContent.childCount - 1; i >= 0; i--)
        {
            Destroy(shopGridContent.GetChild(i).gameObject);
        }

        // สร้าง Slot ใหม่ตามข้อมูลที่มี
        foreach (StationData data in allStations)
        {
            if (data == null) continue;

            GameObject slotObj = Instantiate(shopSlotPrefab, shopGridContent);
            ShopSlotUI slotUI = slotObj.GetComponent<ShopSlotUI>();

            if (slotUI != null)
            {
                bool isOwned = _playerOwnedStations.ContainsKey(data.stationID);
                int level = isOwned ? _playerOwnedStations[data.stationID] : 0;

                slotUI.Setup(data, isOwned, level, OnStationSlotClicked);
            }
        }
    }

    private void OnStationSlotClicked(StationData data)
    {
        if (!data.isUnlocked)
        {
            Debug.Log($"[Shop] {data.stationName} ยังไม่ปลดล็อก");
            return;
        }

        bool isOwned = _playerOwnedStations.ContainsKey(data.stationID);
        int currentLevel = isOwned ? _playerOwnedStations[data.stationID] : 0;

        // สั่งเปิด Popup และส่งฟังก์ชันที่จะให้ทำงานเมื่อกดปุ่ม Buy/Upgrade หรือ Move เข้าไป
        if (detailPanel != null)
        {
            detailPanel.Open(data, isOwned, currentLevel,
                onUpgrade: () => HandleBuyOrUpgrade(data, isOwned, currentLevel),
                onMove: () => HandleMove(data)
            );
        }
    }

    private void HandleBuyOrUpgrade(StationData data, bool isOwned, int currentLevel)
    {
        if (!isOwned)
        {
            // --- กรณีซื้อใหม่ (Lv.1) ---
            int cost = data.GetStatsByLevel(1).upgradeCost;

            if (playerMoney >= cost)
            {
                playerMoney -= cost; // หักเงิน
                UpdateMoneyUI();     // อัปเดตตัวเลขบนจอ

                Debug.Log($"[Shop] ซื้อ {data.stationName} ราคา {cost} สำเร็จ!");
                _playerOwnedStations.Add(data.stationID, 1);

                PopulateShop();
                if (detailPanel != null) detailPanel.Close(); // ซื้อเสร็จปิด Popup

                EnterNewPlacementMode(data);
            }
            else
            {
                Debug.LogWarning("เงินไม่พอซื้อ!");
            }
        }
        else
        {
            // --- กรณีอัปเกรด ---
            int nextLevel = currentLevel + 1;
            if (nextLevel <= 4) // (สมมติ Max Level คือ 4)
            {
                int cost = data.GetStatsByLevel(nextLevel).upgradeCost;

                if (playerMoney >= cost)
                {
                    playerMoney -= cost; // หักเงิน
                    UpdateMoneyUI();     // อัปเดตจอ

                    Debug.Log($"[Shop] อัปเกรด {data.stationName} เป็น Lv.{nextLevel} สำเร็จ!");
                    _playerOwnedStations[data.stationID] = nextLevel;

                    PopulateShop();

                    // สั่งเปิด Popup ใหม่อีกรอบด้วยข้อมูลล่าสุด เพื่อให้เลขเลเวลบนปุ่มอัปเดตทันที
                    OnStationSlotClicked(data);
                }
                else
                {
                    Debug.LogWarning("เงินไม่พออัปเกรด!");
                }
            }
        }
    }

    private void AutoPlaceStation(StationData data)
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.AutoPlaceStation(data);
        }
    }

    private void HandleMove(StationData data)
    {
        // สั่งปิดหน้าต่าง Popup ทันที
        if (detailPanel != null && detailPanel.panelRoot != null)
        {
            detailPanel.panelRoot.SetActive(false);
        }

        EnterMoveMode(data);
    }

    private void EnterMoveMode(StationData data)
    {
        // 1. สั่งให้ UI Shop สไลด์ไปทางซ้ายเพื่อหลบจอ
        SlideUI(false);

        if (MapManager.Instance != null)
        {
            // 2. เรียก MapManager เริ่มโหมดย้าย พร้อมฝากคำสั่งให้สไลด์ UI กลับมาเมื่อวางเสร็จ
            MapManager.Instance.StartMoveMode(data, () =>
            {
                SlideUI(true);
            });
        }
    }

    // ==========================================
    // ส่วนของการจัดการ Slide Animation
    // ==========================================
    private void SlideUI(bool isShowing)
    {
        if (shopPanelRect == null) return;

        if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
        _slideCoroutine = StartCoroutine(SlideRoutine(isShowing));
    }

    private IEnumerator SlideRoutine(bool isShowing)
    {
        Vector2 startPos = shopPanelRect.anchoredPosition;

        // ถ้า isShowing=true ให้กลับมาที่เดิม, ถ้า false ให้ลบตำแหน่ง X ออกไปตาม slideDistance
        Vector2 targetPos = isShowing ? _originalPosition : new Vector2(_originalPosition.x - slideDistance, _originalPosition.y);

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);

            // สูตรคณิตศาสตร์ทำให้กราฟความเร็วการเลื่อนสมูทขึ้น (Ease-In-Out)
            float curve = t * t * (3f - 2f * t);

            shopPanelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, curve);
            yield return null;
        }

        // บังคับจบที่ตำแหน่งเป้าหมายให้แม่นยำ
        shopPanelRect.anchoredPosition = targetPos;
    }
    private void EnterNewPlacementMode(StationData data)
    {
        SlideUI(false); // สไลด์ UI หลบ

        if (MapManager.Instance != null)
        {
            // เรียกโหมดหยิบของใหม่ที่ MapManager
            MapManager.Instance.StartNewPlacementMode(data, () =>
            {
                SlideUI(true); // วางเสร็จค่อยเลื่อน UI กลับมา
            });
        }
    }
}