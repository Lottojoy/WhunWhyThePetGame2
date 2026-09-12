using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerEntry : INetworkSerializable, IEquatable<PlayerEntry>
{
    public ulong ClientId;
    public FixedString64Bytes Name;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Name);
    }

    public bool Equals(PlayerEntry other)
    {
        return ClientId == other.ClientId && Name.Equals(other.Name);
    }

    public override bool Equals(object obj) => obj is PlayerEntry other && Equals(other);
    public override int GetHashCode() => ClientId.GetHashCode();
}

public class LobbyManager : NetworkBehaviour
{
    [Header("=== Player List ===")]
    [SerializeField] private Transform playerListContainer;

    private NetworkList<PlayerEntry> playerEntries;

    void Awake()
    {
        playerEntries = new NetworkList<PlayerEntry>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playerEntries.OnListChanged += OnPlayerListChanged;

        if (IsServer)
        {
            // Add all currently connected clients
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                AddPlayerToServer(client.ClientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        playerEntries.OnListChanged -= OnPlayerListChanged;
    }

    // === Server: manage player entries ===

    private void OnServerClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        AddPlayerToServer(clientId);
    }

    private void OnServerClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;
        RemovePlayerFromServer(clientId);
    }

    private void AddPlayerToServer(ulong clientId)
    {
        // Check if already exists
        for (int i = 0; i < playerEntries.Count; i++)
        {
            if (playerEntries[i].ClientId == clientId) return;
        }

        string playerName = $"Player {clientId}";
        playerEntries.Add(new PlayerEntry
        {
            ClientId = clientId,
            Name = playerName
        });
    }

    private void RemovePlayerFromServer(ulong clientId)
    {
        for (int i = playerEntries.Count - 1; i >= 0; i--)
        {
            if (playerEntries[i].ClientId == clientId)
            {
                playerEntries.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// Server updates a player's name (called via ServerRpc from client).
    /// </summary>
    public void SetPlayerName(ulong clientId, string name)
    {
        for (int i = 0; i < playerEntries.Count; i++)
        {
            if (playerEntries[i].ClientId == clientId)
            {
                playerEntries[i] = new PlayerEntry
                {
                    ClientId = clientId,
                    Name = name
                };
                return;
            }
        }
    }

    // === RPC: client sends name to server ===

    [ServerRpc(RequireOwnership = false)]
    private void RegisterNameServerRpc(FixedString64Bytes playerName, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        SetPlayerName(clientId, playerName.ToString());
    }

    // === Callback: NetworkList changed → update UI ===

    private void OnPlayerListChanged(NetworkListEvent<PlayerEntry> changeEvent)
    {
        RefreshPlayerListUI();
    }

    // === Server hooks for connect/disconnect ===

    private bool subscribedCallbacks = false;

    void OnEnable()
    {
        if (NetworkManager.Singleton != null && !subscribedCallbacks)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnServerClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnServerClientDisconnected;
            subscribedCallbacks = true;
        }
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null && subscribedCallbacks)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnServerClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnServerClientDisconnected;
            subscribedCallbacks = false;
        }
    }

    // === UI ===

    void Start()
    {
        ShowLocalPlayer();
    }

    void Update()
    {
        // Register local player name with server once connected
        if (IsClient && !IsServer && !nameRegistered)
        {
            SendNameOnce();
        }
    }

    private bool nameRegistered = false;
    private void SendNameOnce()
    {
        if (nameRegistered) return;
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsClient) return;

        nameRegistered = true;
        string name = PlayerNameStorage.PlayerName;
        if (string.IsNullOrEmpty(name)) name = "Player";
        RegisterNameServerRpc(name);
    }

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
        if (playerListContainer == null) return;

        ClearSlots();

        ulong localId = NetworkManager.Singleton.LocalClientId;
        ulong serverId = NetworkManager.ServerClientId;

        // Sort: local first, then host, then by ClientId
        var sorted = new List<PlayerEntry>();
        for (int i = 0; i < playerEntries.Count; i++)
            sorted.Add(playerEntries[i]);

        sorted.Sort((a, b) =>
        {
            if (a.ClientId == localId) return -1;
            if (b.ClientId == localId) return 1;
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
                tmp.color = isLocal ? Color.green : Color.white;
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
