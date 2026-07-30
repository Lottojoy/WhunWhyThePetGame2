using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ติด script นี้ไว้ที่ Prefab ของช่องสัตว์แต่ละอันในหน้า Collection
/// โครงสร้าง Prefab แนะนำ: Button (root) > Image (icon) + TMP_Text (name)
/// </summary>
[RequireComponent(typeof(Button))]
public class AnimalSlotUI : MonoBehaviour
{
    [Header("References")]
    public Image iconImage;
    public TMP_Text nameText;

    private Button _button;
    private AnimalData _data;
    private Action<AnimalData> _onClickCallback;

    public void Setup(AnimalData data, Action<AnimalData> onClickCallback)
    {
        _data = data;
        _onClickCallback = onClickCallback;

        if (_button == null) _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(HandleClick);

        Refresh();
    }

    private void Refresh()
    {
        if (_data == null) return;

        if (_data.isUnlocked)
        {
            iconImage.sprite = _data.colorSprite;
            nameText.text = _data.animalName;
        }
        else
        {
            iconImage.sprite = _data.silhouetteSprite;
            nameText.text = "???";
        }
    }

    private void HandleClick()
    {
        _onClickCallback?.Invoke(_data);
    }
}
