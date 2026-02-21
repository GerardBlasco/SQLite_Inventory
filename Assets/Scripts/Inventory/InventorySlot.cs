using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] Item item;
    [SerializeField] int quantity = 0;

    private static InventorySlot selectedSlot;

    public InventorySlot(){}

    public InventorySlot(Item newItem, int quantity)
    {
        item = newItem;
        this.quantity = quantity;
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
    }

    public Item GetItem()
    {
        return item;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public void AddQuantity(int quantity)
    {
        this.quantity += quantity;

        this.quantity = Mathf.Clamp(this.quantity, 0, item.GetMaxStack());
    }

    public void RemoveQuantity(int quantity)
    {
        this.quantity -= quantity;
    }

    public void SetAsFull()
    {
        quantity = item.GetMaxStack();
    }

    public bool IsFull()
    {
        return quantity == item.GetMaxStack();
    }

    public void SetAsSelected()
    {
        selectedSlot = this;
    }

    public bool IsSelected()
    {
        return selectedSlot == this;
    }

    public void UnSelect()
    {
        selectedSlot = null;
    }
}
