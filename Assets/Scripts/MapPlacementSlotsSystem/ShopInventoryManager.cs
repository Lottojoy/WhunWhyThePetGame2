using System.Collections.Generic;
using UnityEngine;

// --- คลาสจำลองข้อมูลที่รอสคริปต์ Player จริงมาแทนที่ ---
[System.Serializable]
public class MockPlayerData
{
    public int money = 5000;
    // เก็บว่ามี Station อะไรบ้าง เลเวลอะไร และวางไปหรือยัง
    public List<OwnedStation> ownedStations = new List<OwnedStation>();
}

[System.Serializable]
public class OwnedStation
{
    public StationData stationData;
    public int currentLevel = 1;
    public bool isPlaced = false;
}
// --------------------------------------------------------

public class ShopInventoryManager : MonoBehaviour
{
    public static ShopInventoryManager Instance;

    [Header("ข้อมูลจำลอง (รอต่อกับ Player จริง)")]
    public MockPlayerData playerData = new MockPlayerData();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // ฟังก์ชันสำหรับปุ่ม "ซื้อ หรือ อัปเกรด" ใน UI
    public void BuyOrUpgradeStation(StationData targetStation)
    {
        // 1. เช็กว่าผู้เล่นมี Station นี้ใน Inventory หรือยัง
        OwnedStation owned = playerData.ownedStations.Find(s => s.stationData == targetStation);

        if (owned == null)
        {
            // --- กรณีซื้อใหม่ (Lv.1) ---
            int cost = targetStation.GetStatsByLevel(1).upgradeCost;
            if (playerData.money >= cost)
            {
                playerData.money -= cost;
                playerData.ownedStations.Add(new OwnedStation { stationData = targetStation, currentLevel = 1 });
                Debug.Log($"ซื้อ {targetStation.stationName} สำเร็จ! เงินเหลือ: {playerData.money}");
            }
            else
            {
                Debug.LogWarning("เงินไม่พอซื้อ!");
            }
        }
        else
        {
            // --- กรณีมีแล้ว ให้กดอัปเกรด (Lv ถัดไป) ---
            if (owned.currentLevel >= 4)
            {
                Debug.Log("เลเวลตันแล้ว (Max Level)!");
                return;
            }

            int nextLevel = owned.currentLevel + 1;
            int upgradeCost = targetStation.GetStatsByLevel(nextLevel).upgradeCost;

            if (playerData.money >= upgradeCost)
            {
                playerData.money -= upgradeCost;
                owned.currentLevel = nextLevel;
                Debug.Log($"อัปเกรด {targetStation.stationName} เป็นเลเวล {nextLevel}! เงินเหลือ: {playerData.money}");
            }
            else
            {
                Debug.LogWarning("เงินไม่พออัปเกรด!");
            }
        }
    }

    // ฟังก์ชันเช็กว่า Station นี้ถูกซื้อไปแล้วหรือยัง (ให้ DragHandler ใช้)
    public bool IsStationOwnedAndNotPlaced(StationData station)
    {
        OwnedStation owned = playerData.ownedStations.Find(s => s.stationData == station);
        return owned != null && !owned.isPlaced;
    }

    public void MarkStationAsPlaced(StationData station)
    {
        OwnedStation owned = playerData.ownedStations.Find(s => s.stationData == station);
        if (owned != null) owned.isPlaced = true;
    }
}