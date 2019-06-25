using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IClickable
{

    public int quantity = 1;

    public void OnLeftClick()
    {

    }

    public void OnRightClickDown()
    {
        Use();
    }

    public void SetQuantityText()
    {
        transform.Find("QuantityText").GetComponent<Text>().text = quantity.ToString();
    }

    public virtual void Use()
    {
        Debug.Log("Used item: " + name);
    }

    public void Deplete()
    {
        quantity--;
        SetQuantityText();
        if (quantity == 0) GameObject.Destroy(this.gameObject);
    }

    public void Add(int amount)
    {
        quantity += amount;
        SetQuantityText();

        //Add limitation here

    }

    // Use this for initialization
    protected virtual void Start()
    {
        SetQuantityText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        InventoryController.Instance.RemoveItem(this);
    }
}