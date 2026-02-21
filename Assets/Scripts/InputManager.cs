using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public InputActionAsset map;

    public InputAction horizontalX_ia;
    public InputAction horizontalZ_ia;
    public InputAction sprint_ia;
    public InputAction toggleInventory_ia;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

        EnableInput();

        horizontalX_ia = map.FindActionMap("Movement").FindAction("HorizontalX");
        horizontalZ_ia = map.FindActionMap("Movement").FindAction("HorizontalZ");
        sprint_ia = map.FindActionMap("Movement").FindAction("Sprint");
        toggleInventory_ia = map.FindActionMap("Inventory").FindAction("Toggle Inventory");
    }

    public void EnableInput()
    {
        map.Enable();
    }

    public void DisableInput()
    {
        map.Disable();
    }
}
