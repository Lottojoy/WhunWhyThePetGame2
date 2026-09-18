using System;
using System.Collections.Generic;
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

public class NetworkStorage : NetworkBehaviour
{
    // === Singleton — access anywhere via NetworkStorage.Instance ===
    public static NetworkStorage Instance { get; private set; }

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
        if (IsServer)
        {
            // Subscribe disconnect callback
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            subscribedCallbacks = true;

            // Add host directly with real name
            ulong localId = NetworkManager.Singleton.LocalClientId;
            string hostName = PlayerNameStorage.PlayerName;
            if (string.IsNullOrEmpty(hostName)) hostName = "Player";
            AddPlayer(localId, hostName);
        }
        // Client: tell server "I joined, my name is..."
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

    /// <summary>Server-only. Add a player if not already present. Use initialName for host.</summary>
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

    /// <summary>Server-only. Remove a player by clientId.</summary>
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

    /// <summary>Server-only. Update a player's display name.</summary>
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

    // === RPC: client tells server "I joined, my name is..." ===

    [ServerRpc(RequireOwnership = false)]
    public void RegisterNameServerRpc(FixedString64Bytes playerName, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"[Server] Client {clientId} joined with name: {playerName}");
        AddPlayer(clientId, playerName.ToString());
    }
}
