using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ต้องใช้ตัวนี้เพื่อเปลี่ยนซีน

// 1. สร้างคลาสสำหรับเก็บข้อมูลแต่ละช่อง (ต้องมี [System.Serializable] เพื่อให้แปลงเป็น JSON ได้)
[System.Serializable]
public class SlotSaveData
{
    public int slotIndex;      // ลำดับช่องใน Map
    public string stationID;   // ชื่อ ID ของ Station ที่วาง
    public float rotationY;    // มุมหมุน (แกน Y)
}

// 2. สร้างคลาสหลักสำหรับจับมัดรวมข้อมูลทั้งหมด
[System.Serializable]
public class ClinicSaveData
{
    public List<SlotSaveData> savedSlots = new List<SlotSaveData>();
}

public class ClinicSaveManager : MonoBehaviour
{
    [Header("ตั้งค่า Scene")]
    [Tooltip("ชื่อซีนถัดไปที่ต้องการให้โหลด (พิมพ์ให้ตรงกับชื่อไฟล์ Scene)")]
    public string nextSceneName = "GameScene";

    // ฟังก์ชันนี้จะเอาไปผูกกับปุ่ม "Play / Next Scene"
    public void SaveAndGoToNextScene()
    {
        if (MapManager.Instance == null)
        {
            Debug.LogError("ไม่พบ MapManager ในฉาก!");
            return;
        }

        ClinicSaveData data = new ClinicSaveData();

        // วนลูปเช็กทุกช่องใน MapManager
        for (int i = 0; i < MapManager.Instance.allSlots.Count; i++)
        {
            MapSlot slot = MapManager.Instance.allSlots[i];

            // ถ้าช่องนั้นมีของวางอยู่ ให้บันทึกข้อมูล
            if (slot.isOccupied && slot.placedModel != null)
            {
                SlotSaveData slotData = new SlotSaveData();
                slotData.slotIndex = i;
                slotData.stationID = slot.currentStationID;
                slotData.rotationY = slot.placedModel.transform.eulerAngles.y; // เก็บมุมองศา

                data.savedSlots.Add(slotData);
            }
        }

        // แปลงข้อมูลเป็นข้อความ JSON
        string json = JsonUtility.ToJson(data);

        // บันทึกลงระบบเครื่องด้วย PlayerPrefs (ตั้งชื่อไฟล์จำลองว่า "ClinicMapSave")
        PlayerPrefs.SetString("ClinicMapSave", json);
        PlayerPrefs.Save();

        Debug.Log("บันทึกตำแหน่ง Station สำเร็จ! ข้อมูลที่เซฟ: " + json);

        // โหลดซีนต่อไป
        SceneManager.LoadScene(nextSceneName);
    }

    /* โค้ดสำหรับใช้อ่านค่าในซีนต่อไป (เขียนไว้ใน Manager ของซีนเกม)
    string json = PlayerPrefs.GetString("ClinicMapSave", "");
if (!string.IsNullOrEmpty(json))
{
    ClinicSaveData data = JsonUtility.FromJson<ClinicSaveData>(json);
    foreach(SlotSaveData slotData in data.savedSlots)
    {
        Debug.Log($"ต้องสร้าง {slotData.stationID} ที่ช่อง {slotData.slotIndex} และหมุน {slotData.rotationY} องศา");
        // วางโค้ด Instantiate โมเดลลงตาม Slot Index ที่นี่
    }
}*/
}