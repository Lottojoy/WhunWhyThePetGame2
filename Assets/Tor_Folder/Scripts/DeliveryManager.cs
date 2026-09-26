using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public event EventHandler OnAnimalSpawned;
    public event EventHandler OnAnimalOrderExpired; // เผื่อใช้ตอนหมดเวลา

    [SerializeField] private AnimalListSO animalListSOs;
    [SerializeField] private PetDifficulty currentDifficulty = PetDifficulty.Easy;

    private List<AnimalOrder> waitingOrderList;
    private float spawnTimer;
    private float spawnTimerMax = 4f;
    private int waitingAnimalMax = 4;

    private void Awake()
    {
        Instance = this;
        waitingOrderList = new List<AnimalOrder>();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnTimerMax;

            if (animalListSOs != null && animalListSOs.animalList.Count > 0 && waitingOrderList.Count < waitingAnimalMax)
            {
                int randomIndex = UnityEngine.Random.Range(0, animalListSOs.animalList.Count);
                AnimalData randomAnimalSO = animalListSOs.animalList[randomIndex];

                List<StationType> shuffledStations = new List<StationType>(randomAnimalSO.usableStations);
                Shuffle(shuffledStations);

                int actionCount = shuffledStations.Count > 0
                    ? UnityEngine.Random.Range(1, shuffledStations.Count + 1)
                    : 0;

                AnimalOrder newOrder = new AnimalOrder
                {
                    animalData = randomAnimalSO,
                    requiredStations = shuffledStations.GetRange(0, actionCount),
                    spawnTime = Time.time,
                    patienceTime = randomAnimalSO.GetPatienceTime(currentDifficulty)
                };

                waitingOrderList.Add(newOrder);

                OnAnimalSpawned?.Invoke(this, EventArgs.Empty);
            }
        }

        // เช็คว่ามี order ไหนหมดเวลาหรือยัง (ถ้าจะทำระบบสัตว์หนีในอนาคต)
        for (int i = waitingOrderList.Count - 1; i >= 0; i--)
        {
            AnimalOrder order = waitingOrderList[i];
            float elapsed = Time.time - order.spawnTime;
            if (elapsed >= order.patienceTime)
            {
                waitingOrderList.RemoveAt(i);
                OnAnimalOrderExpired?.Invoke(this, EventArgs.Empty);
                OnAnimalSpawned?.Invoke(this, EventArgs.Empty); // ให้ UI รีเฟรชด้วย
            }
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public List<AnimalOrder> GetWaitingOrderList()
    {
        return waitingOrderList;
    }
}