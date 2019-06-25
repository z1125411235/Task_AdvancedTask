//if(Input.GetMouseButtonUp(0)){
//	pointerEventData.position = Input.mousePosition;
//	raycastResults.Clear();
//	graphicRaycaster.Raycast(pointerEventData, raycastResults);
//
//	draggedItem.transform.SetParent(draggedItemParent);
//	if(raycastResults.Count > 0){
//
//		foreach(var result in raycastResults){
//			//Skip the result being the dragged item itself
//			if(result.gameObject == draggedItem) continue;
//			//If placed on an empty slot
//			if(result.gameObject.CompareTag("Slot")){
//				draggedItem.transform.SetParent(result.gameObject.transform);
//				break;
//			}
//			//If place on an slot with an item
//			if(result.gameObject.CompareTag("ItemIcon")){
//				//Stack the items
//				if(draggedItem.name == result.gameObject.name){
//					//Destroy dragged Item
//					GameObject.Destroy(draggedItem);
//					//Increase quantity
//					int quantity = result.gameObject.GetComponent<Item>().quantity += draggedItem.GetComponent<Item>().quantity; 
//					result.gameObject.transform.Find("QuantityText").GetComponent<Text>().text = quantity.ToString();
//				}
//				//Swap the items
//				else {
//					draggedItem.transform.SetParent(result.gameObject.transform.parent);
//					result.gameObject.transform.SetParent(draggedItemParent);
//					result.gameObject.transform.localPosition = Vector3.zero;
//					break;
//				}
//			}
//		}
//	}
//	//Reset Position of the item
//	draggedItem.transform.localPosition = Vector3.zero;
//	draggedItem = null;
//}