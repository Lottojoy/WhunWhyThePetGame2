using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    // Event สื่อสารเมื่อมีสัตว์เพิ่มในคิว
    public event EventHandler OnAnimalSpawned;

    [SerializeField] private AnimalListSO animalListSOs;
    private List<AnimalSO> waitingAnimalSOList;
    private float spawnTimer;
    private float spawnTimerMax = 4f;
    private int waitingAnimalMax = 4;

    private void Awake()
    {
        Instance = this;
        waitingAnimalSOList = new List<AnimalSO>();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnTimerMax;

            if (animalListSOs != null && animalListSOs.animalList.Count > 0 && waitingAnimalSOList.Count < waitingAnimalMax)
            {
                AnimalSO randomAnimalSO = animalListSOs.animalList[UnityEngine.Random.Range(0, animalListSOs.animalList.Count)];
                waitingAnimalSOList.Add(randomAnimalSO);

                // แจ้ง UI ว่ามีสัตว์เข้ามาใหม่แล้ว
                OnAnimalSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public List<AnimalSO> GetWaitingAnimalSOList()
    {
        return waitingAnimalSOList;
    }
}