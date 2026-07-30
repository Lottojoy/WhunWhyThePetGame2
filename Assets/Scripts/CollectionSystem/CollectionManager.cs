using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// คุมหน้า UI Collection หลัก
/// - Spawn ช่องสัตว์ทุกตัวเข้า Grid (ต้องตั้ง GridLayoutGroup ที่ gridContent เป็น
///   Constraint = Fixed Column Count, Constraint Count = 2 เพื่อให้ได้แถวละ 2 ตามสเปก)
/// - gridContent ต้องเป็น Content ของ ScrollRect ที่มี Scrollbar แนวตั้งต่ออยู่แล้ว
/// - ปุ่ม Back ของ UI นี้ ให้เรียก CloseCollection()
/// </summary>
public class CollectionManager : MonoBehaviour
{
    [Header("ข้อมูลสัตว์ทั้งหมด (All Animal Data)")]
    public List<AnimalData> animalList = new List<AnimalData>();

    [Header("Grid Setup")]
    [Tooltip("Content object ของ ScrollRect ที่มี GridLayoutGroup ติดอยู่ (Fixed Column Count = 2)")]
    public Transform gridContent;
    public GameObject slotPrefab;

    [Header("References")]
    [Tooltip("Root object ของ UI Collection นี้เอง สำหรับให้ปุ่ม Back ปิดได้")]
    public GameObject collectionPanel;
    public AnimalDetailPanel detailPanel;

    private void OnEnable()
    {
        PopulateGrid();
    }

    private void PopulateGrid()
    {
        // เคลียร์ของเก่าก่อนกันซ้ำ (เผื่อเปิดใหม่หลายรอบ)
        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }

        foreach (AnimalData data in animalList)
        {
            if (data == null) continue;

            GameObject slotObj = Instantiate(slotPrefab, gridContent);
            AnimalSlotUI slotUI = slotObj.GetComponent<AnimalSlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(data, OnAnimalSlotClicked);
            }
            else
            {
                Debug.LogWarning("slotPrefab ไม่มี component AnimalSlotUI ติดอยู่", slotObj);
            }
        }
    }

    private void OnAnimalSlotClicked(AnimalData data)
    {
        if (detailPanel != null)
        {
            detailPanel.Open(data);
        }
    }

    /// <summary>เรียกจากปุ่ม Back ใน Inspector (OnClick)</summary>
    public void CloseCollection()
    {
        if (collectionPanel != null)
        {
            collectionPanel.SetActive(false);
        }
    }
}
