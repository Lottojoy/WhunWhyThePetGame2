using System;
using Unity.Netcode;

//Example Module For NetworkVariable

public struct PlayerScore : INetworkSerializable, IEquatable<PlayerScore>
{
    public ulong ClientId;
    public int Score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Score);
    }

    public bool Equals(PlayerScore other)
    {
        return ClientId == other.ClientId && Score == other.Score;
    }

    public override bool Equals(object obj) => obj is PlayerScore other && Equals(other);
    public override int GetHashCode() => ClientId.GetHashCode();
}
