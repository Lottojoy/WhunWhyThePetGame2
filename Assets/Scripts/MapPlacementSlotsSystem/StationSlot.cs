using UnityEngine;

public class StationSlot : MonoBehaviour
{
    public int slotID; // กำหนด ID ให้แต่ละช่องไม่ซ้ำกัน
    public bool isOccupied = false;
    public GameObject currentStationObj; // เก็บโมเดลที่วางอยู่

    public void PlaceStation(GameObject modelPrefab)
    {
        isOccupied = true;
        currentStationObj = Instantiate(modelPrefab, transform.position, transform.rotation);
        // ทำให้โมเดลเป็นลูกของ Slot จะได้จัดระเบียบง่าย
        currentStationObj.transform.SetParent(this.transform);
    }
}