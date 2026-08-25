using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// วาดกราฟเส้น (line graph) แบบง่ายด้วย UI Image ล้วนๆ ไม่ต้องใช้ LineRenderer
/// ใช้ได้กับ Canvas แบบ Screen Space (Overlay/Camera) ตามปกติของ UI
/// วิธีใช้: แปะสคริปต์นี้ไว้บน object พื้นที่กราฟ (เช่น object "garp" ในรูป)
/// แล้วเตรียม prefab จุดกลม (dotPrefab) กับ prefab เส้นแบน (linePrefab, pivot ซ้าย)
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UILineGraph : MonoBehaviour
{
    [Header("Prefab จุดกราฟ (Image วงกลมเล็กๆ)")]
    public Image dotPrefab;

    [Header("Prefab เส้นเชื่อม (Image สี่เหลี่ยมแบน)")]
    public Image linePrefab;

    [Header("ระยะขอบใน (padding) ของพื้นที่กราฟ")]
    public float padding = 20f;

    [Header("ความหนาเส้น")]
    public float lineThickness = 4f;

    private RectTransform rect;
    private readonly List<GameObject> spawned = new List<GameObject>();

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    /// <summary>วาดกราฟจากค่ารายวัน เช่น [31, 45, 112]</summary>
    public void Draw(List<int> values)
    {
        Clear();
        if (values == null || values.Count == 0) return;

        float width = rect.rect.width - padding * 2f;
        float height = rect.rect.height - padding * 2f;

        int min = values.Min();
        int max = values.Max();
        if (max == min) max = min + 1; // กันหารด้วย 0 ตอนค่าทุกวันเท่ากันหมด

        Vector2[] points = new Vector2[values.Count];
        for (int i = 0; i < values.Count; i++)
        {
            float x = values.Count == 1 ? 0 : (i / (float)(values.Count - 1)) * width;
            float t = (values[i] - min) / (float)(max - min);
            float y = t * height;
            points[i] = new Vector2(-width / 2f + x, -height / 2f + y);
        }

        // เส้นเชื่อมก่อน จะได้อยู่ใต้จุด
        for (int i = 0; i < points.Length - 1; i++)
            DrawLine(points[i], points[i + 1]);

        foreach (var p in points)
            DrawDot(p);
    }

    private void DrawDot(Vector2 pos)
    {
        Image dot = Instantiate(dotPrefab, transform);
        dot.gameObject.SetActive(true); // กันกรณี template ต้นฉบับถูกปิดไว้ไม่ให้โผล่ซ้ำ
        dot.rectTransform.anchoredPosition = pos;
        spawned.Add(dot.gameObject);
    }

    private void DrawLine(Vector2 from, Vector2 to)
    {
        Image line = Instantiate(linePrefab, transform);
        line.gameObject.SetActive(true); // กันกรณี template ต้นฉบับถูกปิดไว้ไม่ให้โผล่ซ้ำ
        RectTransform lr = line.rectTransform;

        Vector2 dir = to - from;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        lr.pivot = new Vector2(0f, 0.5f);
        lr.sizeDelta = new Vector2(distance, lineThickness);
        lr.anchoredPosition = from;
        lr.rotation = Quaternion.Euler(0, 0, angle);

        spawned.Add(line.gameObject);
    }

    private void Clear()
    {
        foreach (var go in spawned)
            if (go != null) Destroy(go);
        spawned.Clear();
    }
}