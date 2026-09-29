using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Station
{
    public string station_id;
    public bool unlock = false;
    public int level;
}

[System.Serializable]
public class SlotSaveData
{
    public int slotIndex;      // ลำดับช่องใน Map
    public string stationID;   // ชื่อ ID ของ Station ที่วาง
    public float rotationY;    // มุมหมุน (แกน Y)
}

[System.Serializable]
public class ClinicSaveData
{
    public string name;
    public int day;
    public int money;
    public int total_earn;
    public List<Station> stations = new();
    public List<SlotSaveData> saved_slots = new();
    public List<int> unlocked_animals = new();
    public List<int> daliy_earnings = new();
}

// Wrapper item to convert KeyValuePair into a serializable struct/class
[System.Serializable]
public struct ClinicMapPair
{
    public int key;
    public ClinicSaveData value;

    public ClinicMapPair(int key, ClinicSaveData value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class ListClinicData : ISerializationCallbackReceiver
{
    // C# usage dictionary (ignored by JsonUtility)
    [System.NonSerialized]
    public Dictionary<int, ClinicSaveData> list = new();

    // Serialized backing list used by JsonUtility
    [SerializeField]
    private List<ClinicMapPair> serializedList = new();

    // Before saving JSON: Dict -> List
    public void OnBeforeSerialize()
    {
        serializedList.Clear();
        foreach (var kvp in list)
        {
            serializedList.Add(new ClinicMapPair(kvp.Key, kvp.Value));
        }
    }

    // After loading JSON: List -> Dict
    public void OnAfterDeserialize()
    {
        list.Clear();
        foreach (var pair in serializedList)
        {
            list[pair.key] = pair.value;
        }
    }
}

public class ClinicSaveManager : MonoBehaviour
{
    public static ClinicSaveManager Instance { get; private set; }

    [Header("ตั้งค่า Scene")]
    [Tooltip("ชื่อซีนถัดไปที่ต้องการให้โหลด (พิมพ์ให้ตรงกับชื่อไฟล์ Scene)")]
    public string nextSceneName = "GameScene";
    private ListClinicData listClinicData = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        string json = PlayerPrefs.GetString("ClinicMapList", "");
        if (!string.IsNullOrEmpty(json))
        {
            listClinicData = JsonUtility.FromJson<ListClinicData>(json);
        }
    }

    public ListClinicData getAllMap()
    {
        return listClinicData;
    }

    public void setMap(int mapId, ClinicSaveData data)
    {
        listClinicData.list[mapId] = data;

        // FIXED: Serializing listClinicData object instead of unsupported listClinicData.list
        string json = JsonUtility.ToJson(listClinicData);

        PlayerPrefs.SetString("ClinicMapList", json);
        PlayerPrefs.Save();

        Debug.Log("บันทึกตำแหน่ง Station สำเร็จ! ข้อมูลที่เซฟ: " + json);
    }

    public void SaveMap(int mapId, ClinicSaveData data)
    {
        if (MapManager.Instance == null)
        {
            Debug.LogError("ไม่พบ MapManager ในฉาก!");
            return;
        }

        // Get existing data or create a new instance if key mapId doesn't exist yet
        if (!listClinicData.list.TryGetValue(mapId, out ClinicSaveData currentData))
        {
            currentData = data ?? new ClinicSaveData();
            listClinicData.list[mapId] = currentData;
        }

        // Clear existing slots to avoid duplicates on re-saving
        currentData.saved_slots.Clear();

        for (int i = 0; i < MapManager.Instance.allSlots.Count; i++)
        {
            MapSlot slot = MapManager.Instance.allSlots[i];

            if (slot.isOccupied && slot.placedModel != null)
            {
                SlotSaveData slotData = new SlotSaveData
                {
                    slotIndex = i,
                    stationID = slot.currentStationID,
                    rotationY = slot.placedModel.transform.eulerAngles.y
                };
                currentData.saved_slots.Add(slotData);
            }
        }

        string json = JsonUtility.ToJson(listClinicData);

        PlayerPrefs.SetString("ClinicMapList", json);
        PlayerPrefs.Save();

        Debug.Log("บันทึกตำแหน่ง Station สำเร็จ! ข้อมูลที่เซฟ: " + json);
    }

    public void SaveAndGoToNextScene()
    {
        if (MapManager.Instance == null)
        {
            Debug.LogError("ไม่พบ MapManager ในฉาก!");
            return;
        }

        ClinicSaveData data = new ClinicSaveData();

        for (int i = 0; i < MapManager.Instance.allSlots.Count; i++)
        {
            MapSlot slot = MapManager.Instance.allSlots[i];

            if (slot.isOccupied && slot.placedModel != null)
            {
                SlotSaveData slotData = new SlotSaveData
                {
                    slotIndex = i,
                    stationID = slot.currentStationID,
                    rotationY = slot.placedModel.transform.eulerAngles.y
                };

                data.saved_slots.Add(slotData);
            }
        }

        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString("ClinicMapSave", json);
        PlayerPrefs.Save();

        Debug.Log("บันทึกตำแหน่ง Station สำเร็จ! ข้อมูลที่เซฟ: " + json);

        SceneManager.LoadScene(nextSceneName);
    }
}