using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputs : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    private PlayerInputsAction playerInputsAction;
    private void Awake()
    {
        playerInputsAction = new PlayerInputsAction();
        playerInputsAction.Player.Enable();

        playerInputsAction.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //if (OnInteractAction != null)
        //    OnInteractAction(this, EventArgs.Empty);
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 getMovementVectorNormalized()
    {
        Vector2 movement = playerInputsAction.Player.Move.ReadValue<Vector2>();
        movement = movement.normalized;
        return movement;
    }
}
