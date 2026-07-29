using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    Vector3 moveDir;
    public CharacterController characterController;
    float playerSpeed = 2;

    public void onMove(InputAction.CallbackContext ctx)
    {
        Vector2 newMoveDir = ctx.ReadValue<Vector2>();
        moveDir.x = newMoveDir.x;
        moveDir.z = newMoveDir.y;
    }

    public void onInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Interact");   
        }
    }

    void Start()
    {
        moveDir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(moveDir * playerSpeed * Time.deltaTime);
    }
}
