using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GenericSlotUI : MonoBehaviour
{
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Button slotButton; // <--- 1. ต้องมีช่องใส่ Button

    private AnimalData _animalData;
    private StationData _stationData;
    private Action<AnimalData> _onAnimalClick;
    private Action<StationData> _onStationClick;

    private void Awake()
    {
        // ดึงคอมโพเนนต์ Button ถ้าไม่ได้ลากใส่ใน Inspector
        if (slotButton == null)
            slotButton = GetComponent<Button>();

        // ดักจับการกดปุ่ม
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    private void OnSlotClicked()
    {
        // ส่งข้อมูลไปยัง Popup
        if (_animalData != null)
        {
            _onAnimalClick?.Invoke(_animalData);
        }
        else if (_stationData != null)
        {
            _onStationClick?.Invoke(_stationData);
        }
    }

    public void Setup(AnimalData data, Action<AnimalData> onClick)
    {
        _animalData = data;
        _stationData = null;
        _onAnimalClick = onClick;
        _onStationClick = null;

        if (data == null) return;

        if (nameText != null) nameText.text = data.isUnlocked ? data.animalName : "???";
        if (displayImage != null) displayImage.sprite = data.isUnlocked ? data.colorSprite : data.silhouetteSprite;
        if (lockedOverlay != null) lockedOverlay.SetActive(!data.isUnlocked);
    }

    public void Setup(StationData data, Action<StationData> onClick)
    {
        _stationData = data;
        _animalData = null;
        _onStationClick = onClick;
        _onAnimalClick = null;

        if (data == null) return;

        if (nameText != null) nameText.text = data.isUnlocked ? data.stationName : "???";
        if (displayImage != null) displayImage.sprite = data.isUnlocked ? data.colorSprite : data.silhouetteSprite;
        if (lockedOverlay != null) lockedOverlay.SetActive(!data.isUnlocked);
    }
}