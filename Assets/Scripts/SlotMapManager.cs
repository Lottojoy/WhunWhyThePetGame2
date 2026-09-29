using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotMapManager : MonoBehaviour
{

    [SerializeField] private Transform slotList;

    void Start()
    {
        ListClinicData listClinicData = ClinicSaveManager.Instance.getAllMap();
        // listClinicData.list
        // ClinicSaveManager.Instance.setMap(0, new ClinicSaveData()
        // {
        //     name = "test"
        // });
        foreach (KeyValuePair<int, ClinicSaveData> pair in listClinicData.list)
        {
            int key = pair.Key;
            ClinicSaveData data = pair.Value;

            Debug.Log($"Key: {key}, Name: {data.name}");
            Transform UsedObject = slotList.GetChild(key).Find("Used");
            GameObject gameObject = UsedObject.gameObject;

            UsedObject.Find("MapnameText (TMP)").gameObject.GetComponent<TMP_Text>().text = data.name;
            gameObject.SetActive(true);
            // slotList.GetChild(key).Find("Used").gameObject.SetActive(true);
        }
        // foreach (Transform slot in slotList)
        // {
        //     slot.Find("Used")
        // }
    }
}
