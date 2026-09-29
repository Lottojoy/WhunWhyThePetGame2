using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Animator animator;
    private const string SPEED = "Speed";
    private const string IS_HOLDING = "IsHolding";
    private const string INTERACT = "Interact";
    [SerializeField] private Player player;
    [SerializeField] private GameInputs gameInputs; // <-- เพิ่มใหม่ ลากตัวเดียวกับที่ Player ใช้ใส่

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (gameInputs != null)
        {
            gameInputs.OnInteractAction += GameInputs_OnInteractAction;
        }
    }

    private void GameInputs_OnInteractAction(object sender, System.EventArgs e)
    {
        animator.SetTrigger(INTERACT);
    }

    private void Update()
    {
        float speed = player.IsWalking() ? 1f : 0f;
        animator.SetFloat(SPEED, speed);

        bool isHolding = player.HasKitchenObject();
        animator.SetBool(IS_HOLDING, isHolding);
    }
}