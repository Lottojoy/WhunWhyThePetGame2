using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindMenu : MonoBehaviour
{
    [SerializeField] private InputActionReference interactRef;

    void OnEnable()
    {
      interactRef.action.Disable();  
    }

    void OnDisable()
    {
      interactRef.action.Enable();    
    }
}
