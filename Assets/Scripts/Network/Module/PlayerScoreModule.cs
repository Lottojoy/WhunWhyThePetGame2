using System;
using Unity.Netcode;
using UnityEngine;

//Example Module For NetworkVariable

public class PlayerScoreModule : INetworkDataModule
{
    private NetworkList<PlayerScore> scores;
    private NetworkStorage storage;

    public NetworkList<PlayerScore> Scores => scores;

    public event Action OnScoresChanged;

    public void OnNetworkSpawn(NetworkStorage storage)
    {
        this.storage = storage;
        scores = new NetworkList<PlayerScore>(
            null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        scores.OnListChanged += HandleChanged;
    }

    public void OnNetworkDespawn()
    {
        if (scores != null)
            scores.OnListChanged -= HandleChanged;
    }

    private void HandleChanged(NetworkListEvent<PlayerScore> e)
    {
        OnScoresChanged?.Invoke();
    }

    // === Server-only methods ===

    public void SetScore(ulong clientId, int score)
    {
        if (!storage.IsServer) return;

        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].ClientId == clientId)
            {
                scores[i] = new PlayerScore { ClientId = clientId, Score = score };
                return;
            }
        }
        scores.Add(new PlayerScore { ClientId = clientId, Score = score });
    }

    public int GetScore(ulong clientId)
    {
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].ClientId == clientId)
                return scores[i].Score;
        }
        return 0;
    }

    public void AddScore(ulong clientId, int amount)
    {
        SetScore(clientId, GetScore(clientId) + amount);
    }
}
