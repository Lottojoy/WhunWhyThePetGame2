using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ConnectRelay : MonoBehaviour
{
    // ✅ Singleton Pattern
    public static ConnectRelay Instance { get; private set; }
    
    private bool _isServicesInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // กันโดนทำลายเวลาเปลี่ยน Scene
    }

    private async void Start()
    {
        await InitializeServices();
    }

    // ✅ ฟังก์ชันนี้จะรันครั้งเดียว และเซฟไว้
    private async Task InitializeServices()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        _isServicesInitialized = true;
        Debug.Log("Relay Services Ready!");
    }

    // ✅ เปลี่ยนจาก static เป็น instance method
    public async Task<string> CreateRelay()
    {
        // ✅ เช็คให้แน่ใจว่าบริการพร้อมก่อนทำงาน
        if (!_isServicesInitialized)
        {
            Debug.LogWarning("Services not ready, waiting...");
            await InitializeServices(); // ถ้ายังไม่พร้อม ก็รอให้พร้อมก่อน
        }

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return "Error";
        }
    }

    // ✅ เปลี่ยนจาก static เป็น instance method
    public async void CloseRelay() // เปลี่ยนจาก async void เป็น async Task (ดีกว่า)
    {
        try
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Relay Closed");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    // ✅ เปลี่ยนจาก static เป็น instance method
    public async Task<bool> JoinRelay(string joinCode) // เปลี่ยนเป็น public เผื่อเรียกใช้
    {
        if (!_isServicesInitialized)
        {
            Debug.LogWarning("Services not ready, waiting...");
            await InitializeServices();
        }

        try
        {
            Debug.Log("Join " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            return NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}