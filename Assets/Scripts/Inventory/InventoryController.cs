using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] InventoryUI ui;
    [SerializeField] public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [SerializeField] int maxSlots = 15;

    private bool isLoading;

    private void Start()
    {
        isLoading = true;
        DataBaseController.LoadDatabaseIntoInventory(this);
        isLoading = false;
    }

    public void AddItem(Item newItem, int quantity)
    {
        bool createAnotherSlot = true;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].GetItem() == newItem && !inventorySlots[i].IsFull())
            {
                createAnotherSlot = false;

                int auxQuantity = (quantity + inventorySlots[i].GetQuantity()) - newItem.GetMaxStack();

                inventorySlots[i].AddQuantity(quantity);

                if (auxQuantity > 0)
                {
                    createAnotherSlot = true;
                    quantity = auxQuantity;
                }
            }
        }

        if (inventorySlots.Count < maxSlots && createAnotherSlot)
        {
            if (quantity > newItem.GetMaxStack())
            {
                quantity -= newItem.GetMaxStack();
                inventorySlots.Add(new InventorySlot(newItem, newItem.GetMaxStack()));
                AddItem(newItem, quantity);
            }
            else
            {
                inventorySlots.Add(new InventorySlot(newItem, quantity));
            }
        }

        ReloadData();
    }

    public void RemoveItem(Item targetItem, int quantity)
    {
        InventorySlot targetSlot = new InventorySlot(targetItem, targetItem.GetMaxStack());

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].GetItem() == targetItem)
            {
                if (targetSlot.GetQuantity() >= inventorySlots[i].GetQuantity())
                {
                    targetSlot = inventorySlots[i];
                }

                //inventorySlots[i].RemoveQuantity(quantity);

                /*if (inventorySlots[i].GetQuantity() <= 0)
                {
                    inventorySlots.RemoveAt(i);
                }*/

                //ui.ReloadInventory();
                //return;
            }
        }

        targetSlot.RemoveQuantity(quantity);

        if (targetSlot.GetQuantity() <= 0)
        {
            inventorySlots.Remove(targetSlot);
        }

        ReloadData();
    }

    public void ReloadData()
    {
        ui.ReloadInventory();

        if (!isLoading)
        {
            isLoading = true;
            DataBaseController.SaveInventoryIntoDatabase(inventorySlots);
            isLoading = false;
        }
    }

    private void OnDestroy()
    {
        DataBaseController.SaveInventoryIntoDatabase(inventorySlots);
    }

    public void DeleteSlot(InventorySlot slot)
    {
        inventorySlots.Remove(slot);
        ReloadData();
    }

    public int GetMaxSlots()
    {
        return maxSlots;
    }

    public int GetCompleteCountFrom(Item item)
    {
        int count = 0;

        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.GetItem() == item)
            {
                count += slot.GetQuantity();
            }
        }

        return count;
    }
}
