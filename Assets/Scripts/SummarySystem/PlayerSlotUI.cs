using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ควบคุมข้อมูลของ PlayerInfo แต่ละช่อง (PlayerInfo 1, 2, 3, 4 ในรูป)
/// วิธีใช้: แปะสคริปต์นี้ไว้บน object PlayerInfo1..4 แต่ละอัน
/// แล้วลาก PlayerImage / PlayerName ของช่องนั้นมาใส่ในช่อง Inspector
/// </summary>
public class PlayerSlotUI : MonoBehaviour
{
    public RawImage playerImage;
    public TMP_Text playerNameText;

    [Tooltip("ถ้าอยากโชว์คะแนน/ยอดขายของ player คนนี้ด้วย ใส่ TMP_Text เพิ่มได้ (ไม่ใส่ก็ได้)")]
    public TMP_Text playerScoreText;

    public void SetData(GameSummaryData.PlayerResult data)
    {
        // แก้ไข: เปลี่ยนจาก .sprite เป็น .texture 
        // และแนะนำให้เปลี่ยนตัวแปรในคลาส GameSummaryData จาก playerSprite เป็น playerTexture (หรือดึง .texture จาก sprite ออกมาใช้)
        if (playerImage != null && data.playerSprite != null)
        {
            // ดึง texture ออกมาจาก sprite ของผู้เล่น
            playerImage.texture = data.playerSprite.texture;
        }

        if (playerNameText != null)
            playerNameText.text = data.playerName;

        if (playerScoreText != null)
            playerScoreText.text = data.score.ToString();
    }

    /// <summary>ถ้าจำนวนผู้เล่นจริงน้อยกว่าช่องที่มี ให้ซ่อนช่องที่เหลือ</summary>
    public void SetEmpty()
    {
        gameObject.SetActive(false);
    }
}
