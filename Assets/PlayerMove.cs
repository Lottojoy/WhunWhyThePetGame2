using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float speed = 7f;
    [SerializeField] private LayerMask countersLayerMask;
    
    private bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;
    private GameInputs gameInputs;

    // NetworkTransform จะถูกเพิ่มใน Inspector
    private NetworkTransform networkTransform;

    private void Awake()
    {
        // ดึง NetworkTransform ที่มีอยู่แล้ว
        networkTransform = GetComponent<NetworkTransform>();
        if (networkTransform == null)
        {
            Debug.LogError("NetworkTransform is missing! Add it to the player prefab.");
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            gameInputs = FindObjectOfType<GameInputs>();
            if (gameInputs != null)
            {
                gameInputs.OnInteractAction += GameInputs_OnInteractAction;
                Debug.Log("PlayerMove: Subscribed to GameInputs for owner " + OwnerClientId);
            }
            else
            {
                Debug.LogError("GameInputs not found in scene!");
            }
        }
        else
        {
            enabled = false; // ปิด Update สำหรับ non-owner
        }
    }

    private void GameInputs_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
            selectedCounter.Interact();
    }

    private void Update()
    {
        if (!IsOwner || gameInputs == null) return;

        HandleMovement();
        HandleInteractions();
    }

    private void HandleMovement()
    {
        Vector2 movement = gameInputs.getMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movement.x, 0, movement.y);

        float moveDistance = speed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }
        if (canMove)
        {
            // ✅ อัปเดต transform.position โดยตรง (NetworkTransform จะซิงค์ให้)
            transform.position += moveDir * moveDistance;
        }
            
        isWalking = moveDir != Vector3.zero;
        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

    private void HandleInteractions()
    {
        Vector2 movement = gameInputs.getMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movement.x, 0, movement.y);

        if (moveDir != Vector3.zero)
            lastInteractDir = moveDir;
        else
            moveDir = lastInteractDir;

        float interactionDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    public bool IsWalking() => isWalking;

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && gameInputs != null)
        {
            gameInputs.OnInteractAction -= GameInputs_OnInteractAction;
        }
    }
}