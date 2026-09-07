using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimalData", menuName = "WHUNWHY/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("ข้อมูลพื้นฐาน (UI & Collection)")]
    public string animalID;
    public string animalName;
    public string unlockCondition; // เช่น "เริ่มเกม (Day 1)", "ต้องมี เคลือบเงา"

    [TextArea(3, 6)]
    public string description;

    public Sprite colorSprite;
    public Sprite silhouetteSprite;
    public GameObject modelPrefab;
    public bool isUnlocked = false;

    [Header("สเตตัสสำหรับ Main Game (Gameplay Stats)")]
    [Tooltip("ระยะเวลาความอดทนก่อนหนี (วินาที) ตามความยาก [0]=Easy, [1]=Medium, [2]=Hard")]
    public float[] patienceTimes = new float[3];

    [Tooltip("เงินโบนัสที่จะได้รับตามความยาก [0]=Easy, [1]=Medium, [2]=Hard")]
    public int[] rewardMoneys = new int[3];

    [Tooltip("Station ที่สัตว์ตัวนี้สามารถใช้งานได้ (ต้องเข้าไปใช้บริการ)")]
    public List<StationType> usableStations = new List<StationType>();

    // ฟังก์ชันช่วยดึงค่าความอดทนตามความยาก
    public float GetPatienceTime(PetDifficulty difficulty)
    {
        return patienceTimes[(int)difficulty];
    }

    // ฟังก์ชันช่วยดึงค่าเงินตามความยาก
    public int GetRewardMoney(PetDifficulty difficulty)
    {
        return rewardMoneys[(int)difficulty];
    }
}