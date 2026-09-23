using UnityEngine;

public class ArrowBobbing : MonoBehaviour
{
    [Header("ตั้งค่าการเคลื่อนไหว")]
    public float bobSpeed = 5f;       // ความเร็วในการเด้งขึ้นลง
    public float bobHeight = 0.5f;    // ระยะความสูงที่เด้ง
    public float rotateSpeed = 90f;   // ความเร็วในการหมุน (เอามาใส่รวมกันได้เลย)

    private Vector3 _startLocalPos;

    private void Start()
    {
        // จำตำแหน่งเริ่มต้นของแกน Y เอาไว้ (ใช้ LocalPosition เพราะลูกศรอาจจะถูกจัดกลุ่มมา)
        _startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        // 1. ทำให้เด้งขึ้นลงด้วยฟังก์ชันคณิตศาสตร์ Sin()
        float newY = _startLocalPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.localPosition = new Vector3(_startLocalPos.x, newY, _startLocalPos.z);

        // 2. สั่งให้หมุนรอบตัวเอง (ถ้าไม่อยากให้หมุน ลบบรรทัดนี้ทิ้งได้)
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}