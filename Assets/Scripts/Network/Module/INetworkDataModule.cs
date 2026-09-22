/// <summary>
/// Implement this to create a network data module that plugs into NetworkStorage.
/// Module owns its own NetworkVariables. NetworkStorage calls lifecycle methods.
/// </summary>
public interface INetworkDataModule
{
    /// <summary>Called when network spawns. Register callbacks here.</summary>
    void OnNetworkSpawn(NetworkStorage storage);

    /// <summary>Called when network despawns. Unregister callbacks here.</summary>
    void OnNetworkDespawn();
}
