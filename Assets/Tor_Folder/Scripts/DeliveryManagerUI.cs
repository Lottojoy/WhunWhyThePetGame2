using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform animalTemplate;

    private void Awake()
    {
        animalTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnAnimalSpawned += DeliveryManager_OnAnimalSpawned;
        UpdateVisual();
    }

    private void DeliveryManager_OnAnimalSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // 1. ลบ UI เก่าที่ไม่ใช่ Template
        foreach (Transform child in container)
        {
            if (child == animalTemplate) continue;
            Destroy(child.gameObject);
        }

        // 2. วนลูปสร้าง UI ใหม่ และส่งข้อมูล AnimalSO ไปแสดงผล
        foreach (AnimalSO animalSO in DeliveryManager.Instance.GetWaitingAnimalSOList())
        {
            Transform animalTransform = Instantiate(animalTemplate, container);
            animalTransform.gameObject.SetActive(true);

            // ดึงสคริปต์ DeliveryManagerSingleUI แล้วส่งข้อมูลสัตว์เข้าไป
            animalTransform.GetComponent<DeliveryManagerSingleUI>().SetAnimalSO(animalSO);
        }
    }
}