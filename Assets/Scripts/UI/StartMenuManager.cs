using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [Header("=== Audio Settings ===")]
    [Tooltip("พิมพ์ชื่อเพลง BGM ที่ตั้งไว้ใน AudioManager ลงช่องนี้")]
    public string menuBgmName = "MenuBackground"; // สร้างตัวแปรให้แก้ใน Inspector ได้

    [Tooltip("ชื่อเสียงตอนกดปุ่ม Play")]
    public string playButtonSfxName = "Click"; // เผื่อไว้ปรับเสียงปุ่มด้วยเลย

    [Header("=== Player Name Input ===")]
    [SerializeField] private TMP_InputField playerNameInput;

    [Header("=== Button ===")]
    [SerializeField] private Button playBtn;

    void Start()
    {
        AudioManager.PlayBGM(menuBgmName);
        playerNameInput.text = "Player";
        playBtn.onClick.AddListener(OnPlayClicked);

    }

    public void OnPlayClicked()
    {
        AudioManager.PlaySFX(playButtonSfxName);

        string name = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
            name = "Player";

        PlayerNameStorage.PlayerName = name;

        GameManager.LoadScene("LobbyScene");
    }
}

// Static class to hold data between scenes
public static class PlayerNameStorage
{
    public static string PlayerName = "Player";
}
