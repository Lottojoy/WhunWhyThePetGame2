using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// แสดงดาว (เต็ม/ครึ่ง/ว่าง) ใต้ object "StarEarn" ในรูป Hierarchy
/// วิธีใช้: วาง RawImage (ดาว 1 ดวง) เป็นลูกของ StarEarn แล้วลาก RawImage นั้นมาใส่ starIconPrefab
/// สคริปต์จะ Instantiate เพิ่มให้ครบตาม maxStars เอง
/// </summary>
public class StarRatingUI : MonoBehaviour
{
    [Header("Prefab ไอคอนดาว 1 ดวง (ต้องมี RawImage component และเป็นลูกของ object นี้)")]
    public RawImage starIconPrefab;

    [Header("จำนวนดาวเต็มทั้งหมด")]
    public int maxStars = 5;

    [Header("Texture ของแต่ละสถานะ (เปลี่ยนจาก Sprite เป็น Texture)")]
    public Texture fullStarTexture;
    public Texture halfStarTexture;
    public Texture emptyStarTexture;

    // แก้ไข: เปลี่ยนชนิดข้อมูลของอาเรย์ให้เป็น RawImage
    private RawImage[] starSlots;

    private void Awake()
    {
        BuildStarSlots();
    }

    private void BuildStarSlots()
    {
        starSlots = new RawImage[maxStars];
        for (int i = 0; i < maxStars; i++)
        {
            RawImage icon = (i == 0) ? starIconPrefab : Instantiate(starIconPrefab, transform);
            icon.gameObject.SetActive(true);
            starSlots[i] = icon;

            // เพิ่มส่วนนี้
            CuteUIAnimator anim = icon.GetComponent<CuteUIAnimator>();
            if (anim == null)
                anim = icon.gameObject.AddComponent<CuteUIAnimator>();

            anim.PopIn(i * 0.15f);
        }
    }

    /// <summary>rating เช่น 3.5 = เต็ม 3 ดวง, ครึ่ง 1 ดวง, ว่าง 1 ดวง</summary>
    public void SetRating(float rating)
    {
        rating = Mathf.Clamp(rating, 0f, maxStars);

        for (int i = 0; i < maxStars; i++)
        {
            float diff = rating - i;

            // แก้ไข: เปลี่ยนจาก .sprite เป็น .texture
            if (diff >= 1f)
                starSlots[i].texture = fullStarTexture;
            else if (diff >= 0.5f)
                starSlots[i].texture = halfStarTexture;
            else
                starSlots[i].texture = emptyStarTexture;
        }
    }
}
