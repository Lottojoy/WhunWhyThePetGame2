using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPount;
    public void Interact()
    {
        Debug.Log("Interacting with ClearCounter");
        Transform kitchenObjectTransfrom = Instantiate(kitchenObjectSO.prefab, counterTopPount);
        kitchenObjectTransfrom.localPosition = Vector3.zero;

        Debug.Log(kitchenObjectTransfrom.GetComponent<KichenObject>().GetKitchenObjectSO().objectName);
    }
}
