using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text quantity;
    [SerializeField] InventoryController inventory;

    InventorySlot currentSelected;

    public void UnSelectPrevious()
    {
        foreach (InventorySlot s in inventory.inventorySlots)
        {
            if (s == currentSelected && currentSelected.IsSelected())
            {
                currentSelected.UnSelect();
            }
        }
    }

    public void ChangeToSelected()
    {
        UnSelectPrevious();

        foreach (InventorySlot s in inventory.inventorySlots)
        {
            if (s.IsSelected())
            {
                //Debug.Log(s.GetItem().GetId());
                currentSelected = s;
                UpdateInfo(s.GetItem());
                return;
            }
        }

        if (!currentSelected.IsSelected())
        {
            icon.color = new Color(0, 0, 0, 0);
            itemName.text = "...";
            quantity.text = "";
        }
    }

    public void UpdateInfo(Item s)
    {
        itemName.text = s.GetID();
        icon.color = new Color(255, 255, 255, 255);
        icon.sprite = s.GetSprite();
        quantity.text = inventory.GetCompleteCountFrom(s) <= 1 ? "" : inventory.GetCompleteCountFrom(s).ToString();
    }

    public void AddItem()
    {
        if (currentSelected == null)
        {
            return;
        }

        inventory.AddItem(currentSelected.GetItem(), 1);
        UpdateInfo(currentSelected.GetItem());
    }

    public void RemoveItem()
    {
        if (currentSelected == null)
        {
            return;
        }

        inventory.RemoveItem(currentSelected.GetItem(), 1);
        UpdateInfo(currentSelected.GetItem());
    }

    public void DeleteSlot()
    {
        if (currentSelected == null)
        {
            return;
        }

        inventory.DeleteSlot(currentSelected);
        UpdateInfo(currentSelected.GetItem());
    }

    public Item GetSelectedItem()
    {
        return currentSelected != null ? currentSelected.GetItem() : null;
    }
}
