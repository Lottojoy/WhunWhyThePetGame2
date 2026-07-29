using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("--------- MenuCanvas ---------")]
    [SerializeField] private GameObject generalSettingMenu;
    [SerializeField] private GameObject controlsMenu;

    [Header("--------- MainButton ---------")]
    [SerializeField] private Button optionsBtn;

    [Header("--------- ChangePageButton ---------")]
    [SerializeField] private Button settingMenuBtn;
    [SerializeField] private Button controlsMenuBtn;

    private GameObject currentMenu;

    public void onBackBtn()
    {
        currentMenu.SetActive(false);
        currentMenu = null;
    }

    void Start()
    {
        optionsBtn.onClick.AddListener(() =>
        {
            currentMenu = generalSettingMenu;
            currentMenu.SetActive(true);
        });
        settingMenuBtn.onClick.AddListener(() =>
        {
            if (currentMenu)
            {
                currentMenu.SetActive(false);
            }
            currentMenu = generalSettingMenu;
            currentMenu.SetActive(true);
        });
        controlsMenuBtn.onClick.AddListener(() =>
        {
            if (currentMenu)
            {
                currentMenu.SetActive(false);
            }
            currentMenu = controlsMenu;
            currentMenu.SetActive(true);
        });
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
