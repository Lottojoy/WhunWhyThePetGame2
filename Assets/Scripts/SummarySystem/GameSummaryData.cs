using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ตอนจบวันในซีนเกมหลัก ก่อน SceneManager.LoadScene("Summary") ให้เรียก:
/// GameSummaryData.Instance.SubmitDayResult(earn: 112, star: 3.5f, playerResults: myPlayerList);
/// </summary>
public class GameSummaryData : MonoBehaviour
{
    public static GameSummaryData Instance { get; private set; }

    [System.Serializable]
    public class PlayerResult
    {
        public string playerName;
        public Sprite playerSprite;   // รูป Player แบบ 2D
        public int score;             // คะแนน/ยอดขายของ Player คนนี้
    }

    [Header("ยอดขาย/รายได้วันนี้")]
    public int todayEarn;

    [Header("ระดับดาวของร้าน (0 - 5, รองรับครึ่งดาว เช่น 3.5)")]
    [Range(0f, 5f)]
    public float starRating;

    [Header("Trend กราฟรายวัน (earn ของแต่ละวันย้อนหลัง)")]
    public List<int> earnTrend = new List<int>();

    [Header("วันปัจจุบัน")]
    public int currentDay = 1;

    [Header("ผลของผู้เล่นแต่ละคน (สูงสุด 4 คนตาม PlayerInfo 1-4)")]
    public List<PlayerResult> players = new List<PlayerResult>();

    [Header("จำนวนวันย้อนหลังสูงสุดที่เก็บไว้ในกราฟ")]
    public int maxTrendHistory = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// เรียกจากซีนเกมหลัก ตอนจบวัน ก่อน LoadScene ไป Summary
    /// เพื่อ push ค่าล่าสุดเข้ามาให้หน้า Summary อ่านต่อ
    /// </summary>
    public void SubmitDayResult(int earn, float star, List<PlayerResult> playerResults)
    {
        todayEarn = earn;
        starRating = star;
        players = playerResults;

        earnTrend.Add(earn);
        if (earnTrend.Count > maxTrendHistory)
            earnTrend.RemoveAt(0);

        currentDay++;
    }
}
