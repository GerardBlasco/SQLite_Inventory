using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private int quantity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<InventoryController>().AddItem(item, quantity);
            Destroy(gameObject);
        }
    }
}
