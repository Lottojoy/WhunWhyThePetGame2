using UnityEngine;

/// <summary>
/// ติดอัตโนมัติกับโมเดล 3D ที่ spawn ในหน้า AnimalDetailPanel
/// ให้หมุนรอบแกน Y ไปเรื่อยๆ เพื่อให้ผู้เล่นเห็นรอบตัวสัตว์
/// </summary>
public class ModelAutoRotate : MonoBehaviour
{
    public float rotateSpeed = 20f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
