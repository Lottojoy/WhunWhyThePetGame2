using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ใช้เทสหน้า Summary แบบเปิดซีนนี้เล่นตรงๆ โดยไม่ต้องผ่านซีนเกมจริง
/// แปะไว้บน Empty GameObject ในซีน Summary เท่านั้น (ลบ/ปิดทิ้งได้ตอนต่อกับเกมจริงแล้ว)
/// ทำงานเฉพาะตอนที่ยังไม่มี GameSummaryData.Instance อยู่ก่อน (กันไม่ให้ทับข้อมูลจริง)
/// </summary>
public class SummaryTestBootstrap : MonoBehaviour
{
    [Header("ลากรูป Player ตัวอย่างมาใส่ไว้เทส (ไม่ใส่ก็ได้)")]
    public List<Sprite> testPlayerSprites;

    private void Awake()
    {
        if (GameSummaryData.Instance != null) return; // มีค่าจริงจากเกมอยู่แล้ว ไม่ต้อง mock ทับ

        GameObject go = new GameObject("GameSummaryData (TEST)");
        var data = go.AddComponent<GameSummaryData>();

        var players = new List<GameSummaryData.PlayerResult>
        {
            new GameSummaryData.PlayerResult
            {
                playerName = "Ohm",
                score = 450,
                playerSprite = testPlayerSprites.Count > 0 ? testPlayerSprites[0] : null
            },
            new GameSummaryData.PlayerResult
            {
                playerName = "Beam",
                score = 320,
                playerSprite = testPlayerSprites.Count > 1 ? testPlayerSprites[1] : null
            },
        };

        // จำลองประวัติกราฟ 4 วันย้อนหลัง (เหมือนในสเก็ตช์ DAY 4: 31, 45, ... 112)
        data.earnTrend = new List<int> { 31, 45, 60 };
        data.SubmitDayResult(earn: 112, star: 3.5f, playerResults: players);
    }
}
