using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject dropdown;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button chefBtn;

    [SerializeField] private GameObject generalSettingMenu;
    [SerializeField] private GameObject rebindMenu;

    [Header("--------- MainButton ---------")]
    [SerializeField] private Button optionsBtn;

    [Header("--------- ChangePageButton ---------")]
    [SerializeField] private Button settingMenuBtn;
    [SerializeField] private Button controlsMenuBtn;

    [Header("--------- PlaySection ---------")]
    [SerializeField] private Button newGameBtn;

    private GameObject currentMenu;

    private Coroutine animationRoutine;

    void Awake()
    {
        dropdown.SetActive(false);
    }

    void Start()
    {
        chefBtn.onClick.AddListener(() =>
        {
            // SceneManager.LoadScene("ChooseAvatar");
            GameManager.LoadScene("ChooseAvatar");
        });

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
            currentMenu = rebindMenu;
            currentMenu.SetActive(true);
        });
        newGameBtn.onClick.AddListener(() =>
        {
            if (!NetworkManager.Singleton.IsServer) return;
            NetworkManager.Singleton.SceneManager.LoadScene("Core_GameScene", LoadSceneMode.Single);
        });
    }

    public void onBackBtn()
    {
        currentMenu.SetActive(false);
        currentMenu = null;
    }

    public void OnPointerEnterPlayBtn()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(ShowDropdown());
    }

    public void OnPointerExitPlayBtn()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(HideDropdown());
    }

    IEnumerator ShowDropdown()
    {
        dropdown.SetActive(true);

        float duration = 0.2f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.SmoothStep(0, 1, t / duration);

            dropdown.transform.localScale = new Vector3(1, value, 1);
            canvasGroup.alpha = value;

            yield return null;
        }

        dropdown.transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
    }

    IEnumerator HideDropdown()
    {
        float duration = 0.2f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.SmoothStep(1, 0, t / duration);

            dropdown.transform.localScale = new Vector3(1, value, 1);
            canvasGroup.alpha = value;

            yield return null;
        }

        dropdown.SetActive(false);
    }
}