using UnityEngine;
using UnityEngine.UI;

public class StarRatingUI : MonoBehaviour
{
    [Header("ดึง RawImage ดาวทั้ง 5 ดวงที่วางตำแหน่งไว้ใน Scene มาใส่ที่นี่")]
    public RawImage[] starSlots;

    [Header("Texture ของแต่ละสถานะ")]
    public Texture fullStarTexture;
    public Texture halfStarTexture;
    // ลบ emptyStarTexture ออกเนื่องจากไม่ได้ใช้งานแล้ว

    private void Awake()
    {
        InitAnimations();
    }

    private void InitAnimations()
    {
        if (starSlots == null) return;

        for (int i = 0; i < starSlots.Length; i++)
        {
            if (starSlots[i] == null) continue;

            CuteUIAnimator anim = starSlots[i].GetComponent<CuteUIAnimator>();
            if (anim == null)
                anim = starSlots[i].gameObject.AddComponent<CuteUIAnimator>();

            anim.PopIn(i * 0.15f);
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

            if (diff >= 1f)
            {
                starSlots[i].gameObject.SetActive(true);
                starSlots[i].texture = fullStarTexture;
            }
            else if (diff >= 0.5f)
            {
                starSlots[i].gameObject.SetActive(true);
                starSlots[i].texture = halfStarTexture;
            }
            else
            {
                // ซ่อนดาวดวงที่เป็นดาวว่าง (ไม่แสดงผล)
                starSlots[i].gameObject.SetActive(false);
            }
        }
    }
}