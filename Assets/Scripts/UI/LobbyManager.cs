using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [Header("=== Player List ===")]
    [SerializeField] private Transform playerListContainer;

    private NetworkStorage networkStorage;

    void Start()
    {
        networkStorage = NetworkStorage.Instance;
        ShowLocalPlayer();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (networkStorage == null)
            networkStorage = NetworkStorage.Instance;

        if (networkStorage != null)
        {
            networkStorage.OnPlayerListChanged += OnPlayerListChanged;

            RefreshPlayerListUI();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (networkStorage != null)
            networkStorage.OnPlayerListChanged -= OnPlayerListChanged;
    }

    // === Callback: data changed → update UI ===

    private void OnPlayerListChanged(NetworkListEvent<PlayerEntry> changeEvent)
    {
        Debug.Log($"[Lobby] Player list changed. Total: {networkStorage.PlayerEntries.Count}");
        RefreshPlayerListUI();
    }

    // === UI ===

    private void ShowLocalPlayer()
    {
        if (playerListContainer == null) return;

        ClearSlots();

        string playerName = PlayerNameStorage.PlayerName;
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player";

        var slot = playerListContainer.GetChild(0);
        var tmp = slot.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = playerName;
            tmp.color = Color.green;
        }
        slot.gameObject.SetActive(true);
    }

    public void RefreshPlayerListUI()
    {
        if (playerListContainer == null || networkStorage == null) return;

        ClearSlots();

        ulong localId = NetworkManager.Singleton.LocalClientId;
        ulong serverId = NetworkManager.ServerClientId;
        var entries = networkStorage.PlayerEntries;

        // Sort: host first, then by ClientId (connection order)
        var sorted = new List<PlayerEntry>();
        for (int i = 0; i < entries.Count; i++)
            sorted.Add(entries[i]);

        sorted.Sort((a, b) =>
        {
            if (a.ClientId == serverId) return -1;
            if (b.ClientId == serverId) return 1;
            return a.ClientId.CompareTo(b.ClientId);
        });

        int slotIndex = 0;
        foreach (var entry in sorted)
        {
            if (slotIndex >= playerListContainer.childCount) break;

            bool isLocal = entry.ClientId == localId;
            bool isHost = entry.ClientId == serverId;

            string displayName = entry.Name.ToString();
            if (isHost)
                displayName += " (Host)";

            var slot = playerListContainer.GetChild(slotIndex);
            var tmp = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = displayName;
                tmp.color = isHost ? Color.green : Color.black;
            }
            slot.gameObject.SetActive(true);
            slotIndex++;
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < playerListContainer.childCount; i++)
        {
            var slot = playerListContainer.GetChild(i);
            var tmp = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "";
            slot.gameObject.SetActive(false);
        }
    }
}
