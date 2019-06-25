using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
     

public class InventoryController : MonoBehaviour {
    public Transform previousParent = null;
    GraphicRaycaster graphRaycaster;
    PointerEventData pointerEventData;
    List<RaycastResult> raycastResults;

    public GameObject draggedItem;

    // Use this for initialization
    void Start () {
        graphRaycaster = UIManager.instance.canvas.GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(null);
        raycastResults = new List<RaycastResult>();
	}
	
	// Update is called once per frame
	void Update () {
        DragItems();
	}

    void DragItems()
    {
        

        if (Input.GetMouseButtonDown(0))
        {
            pointerEventData.position = Input.mousePosition;
            graphRaycaster.Raycast(pointerEventData, raycastResults);
            if(raycastResults.Count > 0)
            {
               if (raycastResults[0].gameObject.GetComponent<Item>())
                {
                    draggedItem = raycastResults[0].gameObject;
                    previousParent = draggedItem.gameObject.transform.parent;
                    draggedItem.transform.SetParent(UIManager.instance.canvas);
                }
            }
        }
        //follow mouse
        if (draggedItem != null)
        {
            draggedItem.GetComponent<RectTransform>().localPosition = UIManager.instance.ScreenToCanvansPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            raycastResults.Clear();
            pointerEventData.position = Input.mousePosition;
            graphRaycaster.Raycast(pointerEventData, raycastResults);
            draggedItem.transform.SetParent(previousParent);
            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    //skip drag when result == dragitem
                    if (result.gameObject == draggedItem) continue;
                    //empty  slot
                    if (result.gameObject.CompareTag("Slot"))
                    {
                        draggedItem.transform.SetParent(result.gameObject.transform);
                        break;
                    }
                    //another item
                    if (result.gameObject.CompareTag("ItemIcon"))
                    {
                        //swap
                        if (result.gameObject.name != draggedItem.gameObject.name)
                        {
                            draggedItem.transform.SetParent(result.gameObject.transform.parent);
                            result.gameObject.transform.SetParent(previousParent);
                            result.gameObject.transform.localPosition = Vector3.zero;
                            break;
                        }
                        else
                        {
                            result.gameObject.GetComponent<Item>().quantity += draggedItem.gameObject.GetComponent<Item>().quantity;
                            result.gameObject.transform.Find("quantityText").gameObject.GetComponent<Text>().text = result.gameObject.GetComponent<Item>().quantity.ToString();
                            GameObject.Destroy(draggedItem);
                            raycastResults.Clear();
                            return;
                        }
                    }
                }
            }
            draggedItem.transform.localPosition = Vector3.zero;
            draggedItem = null;
        }
        raycastResults.Clear();
    }
}
