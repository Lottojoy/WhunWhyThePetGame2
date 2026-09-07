using UnityEngine;

// ประเภทของ Station ทั้งหมดในร้าน
public enum StationType
{
    None,
    Bathing,
    Drying,
    Resting,
    Surgery,
    Polishing
}

// ระดับความยากของสัตว์เลี้ยง
public enum PetDifficulty
{
    Easy = 0,   // Level 1
    Medium = 1, // Level 2
    Hard = 2    // Level 3
}