using System.Collections.Generic;

[System.Serializable]
public class AnimalOrder
{
    public AnimalData animalData;
    public List<StationType> requiredStations;
    public float spawnTime;      // เวลาที่ spawn (Time.time)
    public float patienceTime;   // เวลาทั้งหมดที่มีก่อนหนี (วินาที)
}