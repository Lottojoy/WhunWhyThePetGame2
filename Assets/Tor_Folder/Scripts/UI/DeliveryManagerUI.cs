using System;
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
        foreach (Transform child in container)
        {
            if (child == animalTemplate) continue;
            Destroy(child.gameObject);
        }

        List<AnimalOrder> waitingList = DeliveryManager.Instance.GetWaitingOrderList();

        for (int i = 0; i < waitingList.Count; i++)
        {
            AnimalOrder order = waitingList[i];
            Transform animalTransform = Instantiate(animalTemplate, container);
            animalTransform.gameObject.SetActive(true);

            bool isNewItem = (i == waitingList.Count - 1);

            animalTransform.GetComponent<DeliveryManagerSingleUI>().SetOrder(order, isNewItem);
        }
    }
}