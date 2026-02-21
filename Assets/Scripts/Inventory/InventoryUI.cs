using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform panelTransform;
    [SerializeField] InventoryController inventory;

    public void ReloadInventory()
    {
        foreach (InventorySlotUI s in panelTransform.GetComponentsInChildren<InventorySlotUI>())
        {
            Destroy(s.gameObject);
        }

        foreach (InventorySlot s in inventory.inventorySlots)
        {
            GameObject newSlot = Instantiate(slotPrefab, panelTransform);
            newSlot.GetComponent<InventorySlotUI>().SetInfo(s);
        }

        for (int i = inventory.inventorySlots.Count; i < inventory.GetMaxSlots(); i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, panelTransform);
            newSlot.GetComponent<InventorySlotUI>().SetInfo(null);
        }

        SelectedItemUI selectedItem = FindObjectOfType<SelectedItemUI>();

        if (selectedItem != null && selectedItem.GetSelectedItem() != null)
        {
            selectedItem.UpdateInfo(selectedItem.GetSelectedItem());
        }
    }

    public void Logout()
    {
        PlayerPrefs.SetString("LoggedUser", "");
        SceneManager.LoadScene("MainMenuScene");
    }
}
