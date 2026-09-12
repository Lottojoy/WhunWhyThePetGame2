using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    [Header("=== Panels ===")]
    [SerializeField] private GameObject collectionRootPanel; // หน้าต่าง UI Collection ทั้งหมด (สำหรับปุ่มปิด)
    [SerializeField] private GameObject petPanel;            // ScrollView หรือ Panel ของสัตว์
    [SerializeField] private GameObject stationPanel;        // ScrollView หรือ Panel ของ Station

    [Header("=== Category Buttons ===")]
    [SerializeField] private Button petTabBtn;               // ปุ่มสลับมาดูสัตว์
    [SerializeField] private Button stationTabBtn;           // ปุ่มสลับมาดู Station
    [SerializeField] private Button backBtn;                 // ปุ่มปิด/ย้อนกลับ

    [Header("=== Grid Content Setup ===")]
    [SerializeField] private Transform petGridContent;       // Content ใน ScrollView สัตว์
    [SerializeField] private Transform stationGridContent;   // Content ใน ScrollView Station
    [SerializeField] private GameObject slotPrefab;          // Prefab ช่องแสดงผล (ที่มี GenericSlotUI ติดอยู่)

    [Header("=== References ===")]
    [SerializeField] private CollectionDetailPanel detailPanel; // หน้า Popup รายละเอียด

    [Header("=== Data List ===")]
    public List<AnimalData> animalList = new List<AnimalData>();
    public List<StationData> stationList = new List<StationData>();

    private GameObject currentPanel;

    private void Awake()
    {
        // ตั้งค่าเริ่มต้นให้หมวด Pet เป็น Panel แรก
        currentPanel = petPanel;
    }

    private void Start()
    {
        // ========= ปุ่มหมวด Pet =========
        petTabBtn.onClick.AddListener(() =>
        {
            if (currentPanel == petPanel) return;

            currentPanel.SetActive(false);
            currentPanel = petPanel;
            currentPanel.SetActive(true);
        });

        // ========= ปุ่มหมวด Station =========
        stationTabBtn.onClick.AddListener(() =>
        {
            if (currentPanel == stationPanel) return;

            currentPanel.SetActive(false);
            currentPanel = stationPanel;
            currentPanel.SetActive(true);
        });

        // ========= ปุ่มปิดหน้า Collection =========
        if (backBtn != null)
        {
            backBtn.onClick.AddListener(() =>
            {
                CloseCollection();
            });
        }

        // สร้าง Item ใน Grid ทั้งสองหมวดเตรียมไว้
        PopulatePetGrid();
        PopulateStationGrid();

        // รีเซ็ตหน้าเปิดเริ่มต้น
        ResetToDefaultPanel();
    }

    private void OnEnable()
    {
        // ทุกครั้งที่เปิดหน้า Collection ให้เด้งกลับมาหมวด Pet เสมอ
        ResetToDefaultPanel();
    }

    private void ResetToDefaultPanel()
    {
        if (petPanel == null || stationPanel == null) return;

        petPanel.SetActive(true);
        stationPanel.SetActive(false);
        currentPanel = petPanel;
    }

    // --- ระบบดึง Data เข้า Grid ---

    private void PopulatePetGrid()
    {
        if (petGridContent == null) return;

        foreach (Transform child in petGridContent)
        {
            Destroy(child.gameObject);
        }

        foreach (AnimalData data in animalList)
        {
            if (data == null) continue;

            GameObject slotObj = Instantiate(slotPrefab, petGridContent);
            GenericSlotUI slotUI = slotObj.GetComponent<GenericSlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(data, OnAnimalSlotClicked);
            }
        }
    }

    private void PopulateStationGrid()
    {
        if (stationGridContent == null) return;

        foreach (Transform child in stationGridContent)
        {
            Destroy(child.gameObject);
        }

        foreach (StationData data in stationList)
        {
            if (data == null) continue;

            GameObject slotObj = Instantiate(slotPrefab, stationGridContent);
            GenericSlotUI slotUI = slotObj.GetComponent<GenericSlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(data, OnStationSlotClicked);
            }
        }
    }

    // --- เมื่อคลิกเลือก Slot ---

    private void OnAnimalSlotClicked(AnimalData data)
    {
        if (detailPanel != null)
        {
            detailPanel.Open(data);
        }
    }

    private void OnStationSlotClicked(StationData data)
    {
        if (detailPanel != null)
        {
            detailPanel.Open(data);
        }
    }

    public void CloseCollection()
    {
        if (collectionRootPanel != null)
        {
            collectionRootPanel.SetActive(false);
        }
    }
}