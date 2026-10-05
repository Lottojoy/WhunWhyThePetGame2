using System;
using Unity.Netcode;
using UnityEngine;

public class MoneyManager : NetworkBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public event Action<int> OnMoneyChanged;

    private NetworkVariable<int> money = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        money.OnValueChanged += HandleMoneyChanged;
    }

    public override void OnNetworkDespawn()
    {
        money.OnValueChanged -= HandleMoneyChanged;
    }

    private void HandleMoneyChanged(int previous, int current)
    {
        OnMoneyChanged?.Invoke(current);
    }

    // เรียกได้จากฝั่ง Server เท่านั้น (เช่น ตอน SubmitCounter ส่งงานสำเร็จ)
    public void AddMoney(int amount)
    {
        if (!IsServer) return;
        money.Value += amount;
    }

    public bool TrySpendMoney(int amount)
    {
        if (!IsServer) return false;
        if (money.Value < amount) return false;
        money.Value -= amount;
        return true;
    }

    public int GetMoney()
    {
        return money.Value;
    }

    // ให้ Client เรียกใช้งาน (เช่น กดซื้อของในร้าน) แล้วส่งต่อให้ Server จัดการ
    [ServerRpc(RequireOwnership = false)]
    public void RequestSpendMoneyServerRpc(int amount)
    {
        TrySpendMoney(amount);
    }
}