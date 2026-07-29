using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    [SerializeField] private Button optionsBtn;

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
                    currentMenu = optionsMenu;
                    currentMenu.SetActive(true);
                });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
