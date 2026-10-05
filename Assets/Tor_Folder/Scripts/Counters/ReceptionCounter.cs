using UnityEngine;

public class ReceptionCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO animalPlaceholderSO; // ตัวแทนสัตว์ชั่วคราว

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // ผู้เล่นถือสัตว์อยู่ -> เช็คว่าเสร็จหรือยัง ถ้าเสร็จก็ส่งงาน
            KitchenObject carried = player.GetKitchenObject();
            AnimalOrder order = carried.GetAnimalOrder();

            if (order != null && order.IsFullyComplete())
            {
                DeliveryManager.Instance.CompleteOrder(order);
                Destroy(carried.gameObject);
            }
            else
            {
                Debug.Log("สัตว์ตัวนี้ยังทำงานไม่ครบ ส่งไม่ได้");
            }
        }
        else
        {
            // มือว่าง -> รับสัตว์ตัวแรกสุดในคิวมาถือ
            var waitingList = DeliveryManager.Instance.GetWaitingOrderList();
            if (waitingList.Count == 0) return;

            AnimalOrder order = waitingList[0];

            Transform spawned = Instantiate(animalPlaceholderSO.prefab, player.GetKitchenObjectFollowTranafrom());
            KitchenObject kitchenObject = spawned.GetComponent<KitchenObject>();
            kitchenObject.SetKitchenObjectParent(player);
            kitchenObject.SetAnimalOrder(order);
        }
    }
}