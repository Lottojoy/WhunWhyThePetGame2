using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("--------- MenuCanvas ---------")]
    [SerializeField] private GameObject generalSettingMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject donateMenu;

    [Header("--------- MainButton ---------")]
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button donateBtn;
    [SerializeField] private Button quitBtn;

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
        quitBtn.onClick.AddListener(() =>
        {
           #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif 
        });
        optionsBtn.onClick.AddListener(() =>
        {
            currentMenu = generalSettingMenu;
            currentMenu.SetActive(true);
        });
        donateBtn.onClick.AddListener(() =>
        {
            currentMenu = donateMenu;
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

    // Update is called once per frame
    void Update()
    {

    }
}
