using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text quantity;
    InventorySlot slot;

    private void Start()
    {
        ChangeSelectionColor();
    }

    public void SetInfo(InventorySlot item)
    {
        if (item != null)
        {
            slot = item;
            icon.sprite = item.GetItem().GetSprite();
            quantity.text = item.GetQuantity() <= 1 ? "" : item.GetQuantity().ToString();
        }
        else
        {
            icon.color = new Color(0,0,0,0);
            quantity.text = "";
        }

        ChangeSelectionColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot != null && !slot.IsSelected())
        {
            slot.SetAsSelected();
        }

        FindObjectOfType<SelectedItemUI>().ChangeToSelected();
        FindObjectOfType<InventoryUI>().ReloadInventory();
        ChangeSelectionColor();
    }

    public InventorySlot GetSlot()
    {
        return slot;
    }

    public void ChangeSelectionColor()
    {
        if (slot != null && slot.IsSelected())
        {
            icon.color = Color.red;
        }
        else if (slot != null && !slot.IsSelected())
        {
            icon.color = Color.white;
        }
    }
}
