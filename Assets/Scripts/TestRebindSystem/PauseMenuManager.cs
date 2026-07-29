using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    bool isPause;
    [SerializeField] private GameObject BindingMenu;
    void Start()
    {
        isPause = false;
    }

    public void OnTogglePause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isPause = !isPause;
            if (isPause)
            {
                BindingMenu.SetActive(true);
            }
            else
            {
                BindingMenu.SetActive(false);
            }
        }
    }

    public void OnOkClick()
    {
        isPause = false;
        BindingMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
