using System;
using UnityEngine;

public class WorkStationCounter : BaseCounter
{
    public event Action<float> OnProgressChanged;

    [SerializeField] private StationData stationData;
    [SerializeField] private int currentLevel = 1;

    private float workTimer;
    private bool isWorking;

    public override void Interact(Player player)
    {
        if (isWorking) return; // กำลังทำงานอยู่ แทรกไม่ได้

        if (!HasKitchenObject())
        {
            // รับสัตว์จากมือผู้เล่นมาวางที่ station
            if (player.HasKitchenObject())
            {
                KitchenObject carried = player.GetKitchenObject();
                AnimalOrder order = carried.GetAnimalOrder();

                if (order != null &&
                    order.IsStationRequired(stationData.stationType) &&
                    !order.IsStationCompleted(stationData.stationType))
                {
                    carried.SetKitchenObjectParent(this);
                    StartWork();
                }
            }
        }
        else
        {
            // มีสัตว์อยู่แล้ว (ทำเสร็จแล้ว) ผู้เล่นมาเก็บกลับไป
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    private void StartWork()
    {
        isWorking = true;
        workTimer = 0f;
        OnProgressChanged?.Invoke(0f);
    }

    private void Update()
    {
        if (!isWorking) return;

        StationLevelStats stats = stationData.GetStatsByLevel(currentLevel);
        workTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(workTimer / stats.processTime);
        OnProgressChanged?.Invoke(progress);

        if (workTimer >= stats.processTime)
        {
            CompleteWork();
        }
    }

    private void CompleteWork()
    {
        isWorking = false;
        OnProgressChanged?.Invoke(0f);

        AnimalOrder order = GetKitchenObject().GetAnimalOrder();
        if (order != null)
        {
            order.completedStations.Add(stationData.stationType);
        }
    }
}