using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlotMapManager : MonoBehaviour
{

    public static SlotMapManager Instance { get; private set; }

    [SerializeField] private Transform slotList;

    [SerializeField] public GameObject inputScreenPanel;
    [SerializeField] public Button submitButton;
    [SerializeField] public Button backButton;
    [SerializeField] public TMP_InputField inputField;

    
    public int slotMapId;

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
    }

    private void onSubmit()
    {
        ClinicSaveManager.Instance.SetMap(slotMapId, new ClinicSaveData() {name = inputField.text});
        inputField.text = "";
        inputScreenPanel.SetActive(false);
        GameManager.LoadScene("Core_GameScene");
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        submitButton.onClick.AddListener(onSubmit);
        backButton.onClick.AddListener(() => {
            GameManager.LoadScene("LobbyScene");
        });
    }
}
