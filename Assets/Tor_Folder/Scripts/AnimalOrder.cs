using System.Collections.Generic;

[System.Serializable]
public class AnimalOrder
{
    public AnimalData animalData;
    public List<StationType> requiredStations;
    public HashSet<StationType> completedStations = new HashSet<StationType>();
    public float spawnTime;
    public float patienceTime;

    public bool IsStationRequired(StationType type) => requiredStations.Contains(type);
    public bool IsStationCompleted(StationType type) => completedStations.Contains(type);

    public bool IsFullyComplete()
    {
        foreach (StationType s in requiredStations)
        {
            if (!completedStations.Contains(s)) return false;
        }
        return true;
    }
}