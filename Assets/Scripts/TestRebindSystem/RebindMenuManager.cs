using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindMenuManager : MonoBehaviour
{
    public InputActionReference InteractRef;
    void Start()
    {
        
    }

    private void OnEnable()
    {
      InteractRef.action.Disable();  
    }

    private void OnDisable()
    {
      InteractRef.action.Enable();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
