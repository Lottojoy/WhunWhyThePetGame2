using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

/// <summary>
/// Controller หลักของหน้า Summary
/// ดึงค่าจาก GameSummaryData มาเติมลง UI ทั้งหมด (Earn Text, ดาว, กราฟ, Leaderboard) ตอนเปิดซีน
/// วิธีใช้: แปะสคริปต์นี้ไว้บน object Summary_UICanvas (หรือ BgImage)
/// แล้วลาก reference ต่างๆ มาใส่ตาม Hierarchy ในรูปที่พี่โอมส่งมา
/// </summary>
public class SummaryUIController : MonoBehaviour
{
    [Header("Text แสดงยอดขาย (Earn Text (TMP))")]
    public TMP_Text earnText;

    [Header("แสดงระดับดาว (StarEarn)")]
    public StarRatingUI starRatingUI;

    [Header("กราฟ Trend (garp Text (TMP) (1) หรือ object พื้นที่กราฟ)")]
    public UILineGraph lineGraph;

    [Header("ช่องข้อมูลผู้เล่น เรียงตาม PlayerInfo 1 -> 4")]
    public List<PlayerSlotUI> playerSlots;

    [Header("Text แสดงชื่อ Leader Player (Leader Player Text (TMP) (2))")]
    public TMP_Text leaderPlayerText;

    [Header("เรียง Leaderboard ตามคะแนนมาก -> น้อยหรือไม่")]
    public bool sortByScore = true;

    [Header("ถ้าติ๊กไว้ กราฟจะวาดทันทีตอน Populate (ของเดิม) ถ้าไม่ติ๊ก ต้องให้ SummaryAnimator เรียก DrawGraphAnimated() เอง")]
    public bool autoDrawGraphOnPopulate = false;

    private void Start()
    {
        Populate();
    }

    public void Populate()
    {
        var data = GameSummaryData.Instance;
        if (data == null)
        {
            Debug.LogWarning("[SummaryUIController] ไม่พบ GameSummaryData ในซีน (ยังไม่ได้สร้าง หรือมาเปิดซีน Summary ตรงๆ โดยไม่ผ่านซีนเกม) — ข้ามการเติมค่า");
            return;
        }

        // ยอดขาย
        if (earnText != null)
            earnText.text = $"EARN : {data.todayEarn}";

        // ดาว
        if (starRatingUI != null)
            starRatingUI.SetRating(data.starRating);

        // กราฟ Trend — ถ้าไม่ auto ให้ SummaryAnimator เป็นคนสั่งวาดเองตอนจังหวะที่ต้องการแทน
        if (autoDrawGraphOnPopulate)
            DrawGraphAnimated();

        // Leaderboard
        var results = data.players;
        if (sortByScore)
            results = results.OrderByDescending(p => p.score).ToList();

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i < results.Count)
            {
                playerSlots[i].gameObject.SetActive(true);
                playerSlots[i].SetData(results[i]);
            }
            else
            {
                playerSlots[i].SetEmpty();
            }
        }

        // ผู้เล่นอันดับ 1 (Leader Player)
        if (leaderPlayerText != null && results.Count > 0)
            leaderPlayerText.text = results[0].playerName;
    }

    /// <summary>
    /// วาดกราฟ Trend แบบ animate ทีละจุด เรียกจากภายนอกได้ (เช่นจาก SummaryAnimator
    /// หลังจากกล่องกราฟ PopIn เด้งเข้ามาเสร็จแล้ว) เพื่อให้จังหวะ animation ตรงกัน
    /// </summary>
    public void DrawGraphAnimated()
    {
        var data = GameSummaryData.Instance;
        if (data == null || lineGraph == null) return;

        lineGraph.DrawAnimated(data.earnTrend);
    }
}