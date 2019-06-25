using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{

    [SerializeField] GameObject _itemPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Try to add this item to our inventory
            GameObject newItem = GameObject.Instantiate(_itemPrefab);
            if (InventoryController.Instance.AddItem(newItem))
            {
                GameObject.Destroy(this.gameObject);
            }
            else
            {
                Debug.Log("Inventory is full");
            }
        }
    }
}
