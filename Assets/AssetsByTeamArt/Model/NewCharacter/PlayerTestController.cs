using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerTestController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Animator animator;
    private bool isHoldingItem = false;
    private bool isInteracting = false; // ตัวแปรล็อคสถานะ

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. ตรวจสอบการ Interact (สมมติว่าต้องกดคลิกซ้าย "ค้างไว้" เพื่อทำงาน)
        // Input.GetMouseButton(0) จะเป็น true ตลอดเวลาที่ปุ่มเมาส์ถูกกดค้างอยู่
        if (Input.GetMouseButton(0))
        {
            isInteracting = true;
        }
        else
        {
            isInteracting = false; // ปล่อยเมาส์ปุ๊บ ยกเลิกการทำงานทันที
        }

        // ส่งค่าสถานะไปบอก Animator เพื่อสลับอนิเมชั่น
        animator.SetBool("Interact", isInteracting);

        // 2. ระบบล็อคการเดิน: ถ้ากำลัง Interact อยู่ ให้หยุดเดินและไม่ต้องอ่านโค้ดส่วนเคลื่อนที่
        if (isInteracting)
        {
            animator.SetFloat("Speed", 0f); // บังคับให้หยุดวิ่ง
            return; // จบการทำงานของรอบนี้ทันที โค้ดเดินด้านล่างจะไม่ถูกเรียก
        }

        // 3. ควบคุมการเดินด้วย WASD (จะทำงานก็ต่อเมื่อ isInteracting เป็น false)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            transform.forward = direction;
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // 4. ทดสอบระบบถือของ (คลิกขวา)
        if (Input.GetMouseButtonDown(1))
        {
            isHoldingItem = !isHoldingItem;
            animator.SetBool("IsHolding", isHoldingItem);
        }
    }
}