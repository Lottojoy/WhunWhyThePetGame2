using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // อย่าลืมใส่ Namespace สำหรับ TextMeshPro
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI animalNameText;
    [SerializeField] private Image animalIconImage; // (ตัวเลือกเพิ่มเติม) หากต้องการใส่รูปภาพไอคอนสัตว์

    public void SetAnimalSO(AnimalSO animalSO)
    {
        // กำหนดชื่อสัตว์ลงบนข้อความ UI
        animalNameText.text = animalSO.animalName;

        // (ตัวเลือกเพิ่มเติม) แสดงรูปไอคอนสัตว์ถ้ามีข้อมูล
        if (animalIconImage != null && animalSO.icon != null)
        {
            animalIconImage.sprite = animalSO.icon;
        }
    }
}