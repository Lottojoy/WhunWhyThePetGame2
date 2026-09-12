using System;
using UnityEngine;

public class GameInputs : MonoBehaviour
{
    // ✅ เพิ่ม Instance แบบ Static
    public static GameInputs Instance { get; private set; }

    public event EventHandler OnInteractAction;
    private PlayerInputsAction playerInputsAction;

    private void Awake()
    {
        // ✅ ตั้งค่า Singleton
        if (Instance != null)
        {
            Debug.LogError("มี GameInputs มากกว่า 1 ตัวใน Scene");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputsAction = new PlayerInputsAction();
        playerInputsAction.Player.Enable();
        playerInputsAction.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 getMovementVectorNormalized()
    {
        // ปลอดภัยไว้ก่อน
        if (playerInputsAction == null)
        {
            Debug.LogError("playerInputsAction ยังไม่ถูกสร้าง");
            return Vector2.zero;
        }
        Vector2 movement = playerInputsAction.Player.Move.ReadValue<Vector2>();
        movement = movement.normalized;
        return movement;
    }

    // ✅ ล้าง event เวลา Disable เพื่อป้องกัน Memory Leak
    private void OnDisable()
    {
        if (playerInputsAction != null)
        {
            playerInputsAction.Player.Interact.performed -= Interact_performed;
            playerInputsAction.Disable();
        }
    }
}