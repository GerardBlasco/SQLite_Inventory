using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInventory : MonoBehaviour
{
    InputManager inputManager;

    [SerializeField] GameObject inventoryPanel;

    private void Start()
    {
        inputManager = InputManager.instance;
    }

    void Update()
    {
        if (inputManager.toggleInventory_ia.triggered)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
}
