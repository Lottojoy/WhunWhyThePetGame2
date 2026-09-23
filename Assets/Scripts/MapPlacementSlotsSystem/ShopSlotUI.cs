using UnityEngine;
using UnityEngine.UI;
using TMPro; // เพิ่มบรรทัดนี้เพื่อใช้งาน TextMeshPro

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;   // เปลี่ยนเป็น TextMeshProUGUI
    [SerializeField] private TextMeshProUGUI statusText; // เปลี่ยนเป็น TextMeshProUGUI
    [SerializeField] private Button slotButton;

    private StationData _currentData;
    private System.Action<StationData> _onClickCallback;

    public void Setup(StationData data, bool isOwned, int currentLevel, System.Action<StationData> onClick)
    {
        _currentData = data;
        _onClickCallback = onClick;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnSlotClicked);

        if (!data.isUnlocked)
        {
            // กรณียังไม่ปลดล็อก
            iconImage.sprite = data.silhouetteSprite;
            nameText.text = "???";
            statusText.text = "Locked";
            statusText.color = Color.red;
        }
        else
        {
            // กรณีปลดล็อกแล้ว
            iconImage.sprite = data.colorSprite;
            nameText.text = data.stationName;

            if (isOwned)
            {
                statusText.text = $"Lv.{currentLevel}";
                statusText.color = Color.blue; // สีน้ำเงินบ่งบอกว่ามีแล้ว
            }
            else
            {
                statusText.text = $"Buy: {data.GetStatsByLevel(1).upgradeCost}";
                statusText.color = new Color(1f, 0.5f, 0f); // สีส้ม
            }
        }
    }

    private void OnSlotClicked()
    {
        _onClickCallback?.Invoke(_currentData);
    }
}