using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string id = "Item";
    [SerializeField] private string description = "Description";
    [SerializeField] private int maxStack = 64;

    public Sprite GetSprite()
    {
        return icon;
    }

    public int GetMaxStack()
    {
        return maxStack;
    }

    public string GetID()
    {
        return id;
    }
}
