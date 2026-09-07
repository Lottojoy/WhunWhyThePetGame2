using UnityEngine;

// Struct สำหรับเก็บข้อมูลที่เปลี่ยนไปตามเลเวลของ Station
[System.Serializable]
public struct StationLevelStats
{
    [Header("Level Info")]
    public int level; // 1, 2, 3, 4
    public int upgradeCost; // ราคาอัปเกรด (Lv.1 คือราคาซื้อครั้งแรก)

    [Header("Performance Stats")]
    public float processTime; // ระยะเวลาใช้งาน (วินาที)
    public float satisfactionBonus; // ความพอใจที่เพิ่มขึ้น (วินาที)
    public int serviceFee; // ยอดเงินที่จะได้จากค่าบริการ
}

[CreateAssetMenu(fileName = "NewStationData", menuName = "WHUNWHY/Station Data")]
public class StationData : ScriptableObject
{
    [Header("ข้อมูลพื้นฐาน (Basic Info)")]
    public string stationID;
    public StationType stationType;
    public string stationName;
    public string unlockCondition; // เงื่อนไขปลดล็อคให้ซื้อ Lv.1 ได้

    [TextArea(3, 6)]
    public string description;

    [Header("ระบบการเล่น (Gameplay Setup)")]
    [Tooltip("จำเป็นต้องมีผู้เล่น (สัตวแพทย์) ไปยืนกดทำงานหรือไม่?")]
    public bool requiresPlayerInteraction;

    [Header("ข้อมูลแต่ละเลเวล (Lv.1 - Lv.4)")]
    [Tooltip("ใส่ข้อมูล 4 ช่อง สำหรับ Level 1 ถึง 4")]
    public StationLevelStats[] levelStats = new StationLevelStats[4];

    // ฟังก์ชันช่วยดึงข้อมูลตามเลเวลที่ส่งเข้ามา (level 1-4)
    public StationLevelStats GetStatsByLevel(int currentLevel)
    {
        // ป้องกัน Error หากส่งเลเวลเกิน ให้ใช้เลเวลสูงสุดแทน
        int index = Mathf.Clamp(currentLevel - 1, 0, levelStats.Length - 1);
        return levelStats[index];
    }
}