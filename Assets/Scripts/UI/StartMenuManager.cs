using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [Header("=== Player Name Input ===")]
    [SerializeField] private TMP_InputField playerNameInput;

    [Header("=== Button ===")]
    [SerializeField] private Button playBtn;

    void Start()
    {
        playerNameInput.text = "Player";
        playBtn.onClick.AddListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
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
