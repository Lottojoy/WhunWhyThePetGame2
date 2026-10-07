using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// คุมซีน SUMMARY: แสดงคะแนน earn เป็นตัวเลข + แถบเติม + ดาว 5 ดวง
/// แล้วไปซีนถัดไปเมื่อกดปุ่ม Next
/// </summary>
public class SummaryManager : MonoBehaviour
{
    [Header("ค่าคะแนน (ชั่วคราวสำหรับเทส)")]
    [Tooltip("คะแนนที่ผู้เล่นได้ในด่านนี้ ภายหลังเมื่อระบบเกมหลักเสร็จ ค่อยเปลี่ยนมาดึงค่าจริงแทนการกรอกในนี้ตรงๆ")]
    public int currentEarn = 0;
    [Tooltip("คะแนนเต็มของด่านนี้ ใช้คำนวณ % ของแถบและดาว (ดาวเต็ม 5 ดวง = ได้ครบ maxEarn)")]
    public int maxEarn = 100;

    [Header("Earn Text")]
    public TMP_Text earnText;
    [Tooltip("รูปแบบข้อความ ใช้ {0} แทนตัวเลขคะแนน เช่น EARN : 000")]
    public string earnTextFormat = "EARN : {0:000}";

    [Header("Earn Bar (Image Type ต้องตั้งเป็น Filled, Fill Method = Horizontal)")]
    public Image earnBarFill;
    public bool animateOnStart = true;
    public float animateDuration = 1f;

    [Header("Star Rating")]
    public StarRatingUI starRating;

    [Header("Navigation")]
    [Tooltip("ชื่อซีนถัดไป ต้องถูกเพิ่มใน Build Settings ก่อน (File > Build Settings)")]
    public string nextSceneName;

    private void Start()
    {
        float targetPercent = maxEarn > 0 ? Mathf.Clamp01((float)currentEarn / maxEarn) : 0f;

        if (animateOnStart)
        {
            StartCoroutine(AnimateSummary(targetPercent));
        }
        else
        {
            ApplyInstant(targetPercent);
        }
    }

    private void ApplyInstant(float percent)
    {
        SetEarnText(currentEarn);
        if (earnBarFill != null) earnBarFill.fillAmount = percent;
        if (starRating != null) starRating.SetRating(percent * 5f);
    }

    private void SetEarnText(int value)
    {
        if (earnText != null)
        {
            earnText.text = string.Format(earnTextFormat, value);
        }
    }

    private IEnumerator AnimateSummary(float targetPercent)
    {
        SetEarnText(0);
        if (earnBarFill != null) earnBarFill.fillAmount = 0f;
        if (starRating != null) starRating.SetRating(0f);

        float elapsed = 0f;
        while (elapsed < animateDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animateDuration);

            float currentPercent = Mathf.Lerp(0f, targetPercent, t);
            int displayEarn = Mathf.RoundToInt(Mathf.Lerp(0, currentEarn, t));

            SetEarnText(displayEarn);
            if (earnBarFill != null) earnBarFill.fillAmount = currentPercent;
            if (starRating != null) starRating.SetRating(currentPercent * 5f);

            yield return null;
        }

        // เซ็ตค่าสุดท้ายให้ชัวร์ กันเศษปัดตกหล่นจาก Lerp
        ApplyInstant(targetPercent);
    }

    /// <summary>เรียกจากปุ่ม Next (OnClick ใน Inspector)</summary>
    public void GoToNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("[SummaryManager] ยังไม่ได้ตั้งชื่อ nextSceneName ใน Inspector");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
