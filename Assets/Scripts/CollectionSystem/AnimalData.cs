using UnityEngine;

/// <summary>
/// ScriptableObject เก็บข้อมูลสัตว์แต่ละตัวใน Collection
/// สร้างไฟล์ข้อมูลจริงผ่านเมนู: Assets > Create > Collection > Animal Data
/// </summary>
[CreateAssetMenu(fileName = "NewAnimalData", menuName = "Collection/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("ข้อมูลพื้นฐาน (Basic Info)")]
    public string animalID;
    public string animalName;

    [TextArea(3, 6)]
    public string description;

    [Header("รูปภาพ (Sprites)")]
    [Tooltip("รูปสีตอนปลดล็อคแล้ว (Colored sprite when unlocked)")]
    public Sprite colorSprite;

    [Tooltip("รูปเงาดำตอนยังไม่ปลดล็อค (Silhouette sprite when locked)")]
    public Sprite silhouetteSprite;

    [Header("โมเดล 3D (3D Model)")]
    [Tooltip("Prefab โมเดล 3D ที่จะโชว์ในหน้ารายละเอียด")]
    public GameObject modelPrefab;

    [Header("สถานะปลดล็อค (Unlock State)")]
    [Tooltip("ตัวแปรชั่วคราวสำหรับทดสอบ UI นี้ก่อน ภายหลังเมื่อระบบ Unlock หลักเสร็จ " +
             "ค่อยเปลี่ยนมาอ่านค่าจากระบบเซฟจริงแทนการติ๊กในนี้ตรงๆ")]
    public bool isUnlocked = false;
}
