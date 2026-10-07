using UnityEngine;
using UnityEngine.UI;
using System.Collections; // จำเป็นต้องมีสำหรับ IEnumerator

public class StarRatingUI : MonoBehaviour
{
    [Header("ดึง RawImage ดาวทั้ง 5 ดวงที่วางตำแหน่งไว้ใน Scene มาใส่ที่นี่")]
    public RawImage[] starSlots;

    [Header("Texture ของแต่ละสถานะ")]
    public Texture fullStarTexture;
    public Texture halfStarTexture;

    public string StarSoundSfxName = "StarSound";

    private void Awake()
    {
        InitAnimations();
    }

    private void InitAnimations()
    {
        if (starSlots == null) return;

        // ในส่วนนี้เราจะแค่เตรียม Component CuteUIAnimator ให้ดาวทุกดวงพร้อมใช้งาน
        // แต่ยัง "ไม่สั่งเล่นอนิเมชันหรือเสียง" ในตอนนี้นะครับ
        for (int i = 0; i < starSlots.Length; i++)
        {
            if (starSlots[i] == null) continue;

            CuteUIAnimator anim = starSlots[i].GetComponent<CuteUIAnimator>();
            if (anim == null)
                starSlots[i].gameObject.AddComponent<CuteUIAnimator>();
        }
    }

    /// <summary>rating เช่น 3.5 = เต็ม 3 ดวง, ครึ่ง 1 ดวง, ที่เหลือซ่อน</summary>
    public void SetRating(float rating)
    {
        if (starSlots == null || starSlots.Length == 0) return;

        rating = Mathf.Clamp(rating, 0f, starSlots.Length);

        for (int i = 0; i < starSlots.Length; i++)
        {
            if (starSlots[i] == null) continue;

            float diff = rating - i;
            float delay = i * 0.15f; // สร้างตัวแปร delay คำนวณเวลาหน่วงตามลำดับดาว

            if (diff >= 1f)
            {
                // กรณีดาวเต็มดวง
                starSlots[i].gameObject.SetActive(true);
                starSlots[i].texture = fullStarTexture;

                // สั่งเด้งและเล่นเสียง
                starSlots[i].GetComponent<CuteUIAnimator>().PopIn(delay);
                StartCoroutine(PlaySoundDelayed(StarSoundSfxName, delay));
            }
            else if (diff >= 0.5f)
            {
                // กรณีดาวครึ่งดวง
                starSlots[i].gameObject.SetActive(true);
                starSlots[i].texture = halfStarTexture;

                // สั่งเด้งและเล่นเสียง
                starSlots[i].GetComponent<CuteUIAnimator>().PopIn(delay);
                StartCoroutine(PlaySoundDelayed(StarSoundSfxName, delay));
            }
            else
            {
                // กรณีดาวว่าง ให้ซ่อนไว้ -> ไม่ต้องเด้งและไม่ต้องมีเสียง
                starSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator PlaySoundDelayed(string soundName, float delayTime)
    {
        // 1. รอเวลาตามค่า delay ก่อน
        if (delayTime > 0f)
        {
            yield return new WaitForSeconds(delayTime);
        }

        // 2. เมื่อรอครบเวลาแล้ว ค่อยสั่งเล่นเสียง (ให้อยู่นอกปีกกา if)
        AudioManager.PlaySFX(soundName);
    }
}