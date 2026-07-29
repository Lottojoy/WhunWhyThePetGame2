using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstTimePlayManager : MonoBehaviour
{
    [SerializeField] private GameObject firstTimePlayMenu;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button submitBtn;

    void Start()
    {
        if (PlayerPrefs.GetInt("isPlayed") == 0)
        {
            firstTimePlayMenu.SetActive(true);
            PlayerPrefs.SetInt("isPlayed", 1);
        }
        submitBtn.onClick.AddListener(() =>
        {
            if (nameInput.text.Trim().Length != 0)
            {
                PlayerPrefs.SetString("name", nameInput.text.Trim());
                firstTimePlayMenu.SetActive(false);
            }
        });
    }
}
