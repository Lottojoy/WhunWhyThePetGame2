using UnityEngine;

public class MapSlot : MonoBehaviour
{
    [Header("สถานะของช่องนี้")]
    public bool isOccupied = false;
    public string currentStationID = "";
    public GameObject placedModel;

    [Header("Empty Station Setup")]
    public GameObject emptyStationPrefab; // ลาก station_empty_Prefab มาใส่ที่นี่
    private GameObject _emptyInstance;

    private void Start()
    {
        // ตอนเริ่มเกม ถ้าช่องว่าง ให้เสกโมเดล empty ขึ้นมา
        if (!isOccupied && emptyStationPrefab != null)
        {
            ShowEmptyModel();
        }
    }

    public void ShowEmptyModel()
    {
        if (_emptyInstance == null && emptyStationPrefab != null)
        {
            // สร้างกล่องเปล่าให้เป็นลูกของ Slot
            _emptyInstance = Instantiate(emptyStationPrefab, transform.position, transform.rotation, transform);

            // [เพิ่มบรรทัดนี้] บังคับให้ Scale กลับมาเป็น 1 ไม่ว่าจะโดนแม่บีบแค่ไหนก็ตาม
            _emptyInstance.transform.localScale = Vector3.one;
        }
        if (_emptyInstance != null) _emptyInstance.SetActive(true);
    }

    public void SetStation(StationData data, GameObject model)
    {
        isOccupied = true;
        currentStationID = data.stationID;
        placedModel = model;

        placedModel.transform.position = transform.position;
        placedModel.transform.SetParent(this.transform);

        // [เพิ่มบรรทัดนี้] เพื่อป้องกันโมเดล Station จริงแบนไปด้วย
        placedModel.transform.localScale = Vector3.one;

        HideEmptyModel();
    }
    public void HideEmptyModel()
    {
        if (_emptyInstance != null) _emptyInstance.SetActive(false);
    }

    

    // ฟังก์ชันสำหรับเคลียร์ช่องให้ว่าง (เวลากดย้าย)
    public void ClearSlot()
    {
        isOccupied = false;
        currentStationID = "";
        placedModel = null;

        ShowEmptyModel(); // โชว์กล่องเปล่ากลับมา
    }
}