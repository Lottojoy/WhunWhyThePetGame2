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

    [Header("ค่ารวมของวัน (ปรับได้จาก Inspector)")]
    public int testEarn = 112;
    public float testStarRating = 3.5f;

    [Header("กราฟย้อนหลัง (ปรับจำนวนวัน/ค่าได้เอง)")]
    public List<int> testEarnTrend = new List<int> { 31, 45, 60 };

    [Header("ผู้เล่นทดสอบ (แก้ชื่อ/คะแนนได้ตรงนี้)")]
    public List<TestPlayerEntry> testPlayers = new List<TestPlayerEntry>
    {
        new TestPlayerEntry { playerName = "Ohm", score = 450 },
        new TestPlayerEntry { playerName = "Beam", score = 320 },
        new TestPlayerEntry { playerName = "Lotto", score = 320 },
    };

    [System.Serializable]
    public class TestPlayerEntry
    {
        public string playerName;
        public int score;
        public int spriteIndex = -1; // -1 = ไม่ใช้รูป, อ้างอิง index ใน testPlayerSprites
    }

    private void Awake()
    {
        if (GameSummaryData.Instance != null) return; // มีค่าจริงจากเกมอยู่แล้ว ไม่ต้อง mock ทับ

        SpawnTestData();
    }

    /// <summary>เรียกจากปุ่ม/Context Menu เพื่อสร้างข้อมูลทดสอบตามค่าที่ตั้งไว้ใน Inspector</summary>
    [ContextMenu("Spawn Test Data")]
    [ContextMenu("Spawn Test Data")]
    public void SpawnTestData()
    {
        GameObject go = new GameObject("GameSummaryData (TEST)");
        var data = go.AddComponent<GameSummaryData>();

        var players = new List<GameSummaryData.PlayerResult>();
        for (int i = 0; i < testPlayers.Count; i++)
        {
            var p = testPlayers[i];

            // ถ้าไม่ได้กำหนด spriteIndex ไว้เอง (ยังเป็น -1) ให้ใช้ลำดับ i แทนอัตโนมัติ
            int idx = p.spriteIndex >= 0 ? p.spriteIndex : i;

            Sprite sprite = null;
            if (idx >= 0 && idx < testPlayerSprites.Count)
                sprite = testPlayerSprites[idx];

            players.Add(new GameSummaryData.PlayerResult
            {
                playerName = p.playerName,
                score = p.score,
                playerSprite = sprite
            });
        }

        data.earnTrend = new List<int>(testEarnTrend);
        data.SubmitDayResult(earn: testEarn, star: testStarRating, playerResults: players);
    }
}