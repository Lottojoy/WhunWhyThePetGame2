using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkStorage : NetworkBehaviour
{
    // === Singleton — access anywhere via NetworkStorage.Instance ===
    public static NetworkStorage Instance { get; private set; }

    // === Module system — register INetworkDataModule to get lifecycle hooks ===
    private readonly List<INetworkDataModule> modules = new List<INetworkDataModule>();

    /// <summary>Register a module. Call in Awake or before OnNetworkSpawn.</summary>
    public void RegisterModule(INetworkDataModule module)
    {
        if (!modules.Contains(module))
            modules.Add(module);
    }

    /// <summary>Unregister a module.</summary>
    public void UnregisterModule(INetworkDataModule module)
    {
        modules.Remove(module);
    }

    // === Player data (built-in) ===
    private NetworkList<PlayerEntry> playerEntries = new NetworkList<PlayerEntry>(
        null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    /// <summary>Public read-only access for UI and other systems.</summary>
    public NetworkList<PlayerEntry> PlayerEntries => playerEntries;

    /// <summary>Fired on all clients whenever the player list changes.</summary>
    public event Action<NetworkListEvent<PlayerEntry>> OnPlayerListChanged;

    private bool subscribedCallbacks = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerEntries.OnListChanged += HandleListChanged;
        Debug.Log($"[NS] OnNetworkSpawn — IsServer={IsServer}, IsClient={IsClient}, LocalClientId={NetworkManager.Singleton.LocalClientId}");

        // Notify all registered modules
        for (int i = 0; i < modules.Count; i++)
            modules[i].OnNetworkSpawn(this);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            subscribedCallbacks = true;

            ulong localId = NetworkManager.Singleton.LocalClientId;
            string hostName = PlayerNameStorage.PlayerName;
            if (string.IsNullOrEmpty(hostName)) hostName = "Player";
            AddPlayer(localId, hostName);
        }
        if (IsClient && !IsServer)
        {
            Debug.Log("Isclient");
            string myName = PlayerNameStorage.PlayerName;
            if (string.IsNullOrEmpty(myName)) myName = "Player";
            RegisterNameServerRpc(myName);
            Debug.Log($"[Client] Telling server: my name is {myName}");
        }
    }

    public override void OnNetworkDespawn()
    {
        // Notify all registered modules
        for (int i = 0; i < modules.Count; i++)
            modules[i].OnNetworkDespawn();

        playerEntries.OnListChanged -= HandleListChanged;

        if (IsServer && subscribedCallbacks)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            subscribedCallbacks = false;
        }

        if (Instance == this) Instance = null;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[Server] Client {clientId} disconnected.");
        RemovePlayer(clientId);
    }

    private void HandleListChanged(NetworkListEvent<PlayerEntry> changeEvent)
    {
        OnPlayerListChanged?.Invoke(changeEvent);
    }

    // === Server: manage player entries ===

    public void AddPlayer(ulong clientId, string initialName = null)
    {
        if (!IsServer) return;

        for (int i = 0; i < playerEntries.Count; i++)
        {
            if (playerEntries[i].ClientId == clientId) return;
        }

        string playerName = string.IsNullOrEmpty(initialName) ? $"Player {clientId}" : initialName;
        Debug.Log($"[Server] Adding player: {playerName} (ClientId: {clientId})");
        playerEntries.Add(new PlayerEntry
        {
            ClientId = clientId,
            Name = playerName
        });
    }

    public void RemovePlayer(ulong clientId)
    {
        if (!IsServer) return;

        for (int i = playerEntries.Count - 1; i >= 0; i--)
        {
            if (playerEntries[i].ClientId == clientId)
            {
                playerEntries.RemoveAt(i);
                return;
            }
        }
    }

    public void SetPlayerName(ulong clientId, string name)
    {
        if (!IsServer) return;

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

    [ServerRpc(RequireOwnership = false)]
    public void RegisterNameServerRpc(FixedString64Bytes playerName, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"[Server] Client {clientId} joined with name: {playerName}");
        AddPlayer(clientId, playerName.ToString());
    }
}
