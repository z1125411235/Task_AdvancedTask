using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    // Use this for initialization

    public Transform canvas;
    //Quest Info
    public Transform questInfo;
    public Transform questInfoContent;
    public Transform questInfoQuestName;
    public Button questInfoAcceptButton;
    public Button questInfoCancelButton;
    public Button questInfoCompleteButton;
    //Quest Book - Quest Grid
    public Transform questBook;
    public Transform questBookContent;
    public Button questBookCloseButton;

    public Transform LevelUPText;

    public Transform inventory;
    public Button inventoryCloseButton;
    private void Awake()
    {
        if (instance == null) instance = this;
        
        canvas = GameObject.Find("Canvas").transform;

        questInfo = canvas.Find("Quest Info");
        questInfoContent = questInfo.Find("background/infor/Viewport/Content");
        questInfoQuestName = questInfo.Find("background/Name");
        questInfoAcceptButton = questInfo.Find("background/Buttons/Accept").GetComponent<Button>();

        //cancel
        questInfoCancelButton = questInfo.Find("background/Buttons/Cancel").GetComponent<Button>();
        questInfoCancelButton.onClick.AddListener(() => {
            questInfo.gameObject.SetActive(false);
        });

        questBook = canvas.Find("questBook");
        questBookContent = questBook.Find("background/infor/Viewport/Content");
        questBookCloseButton = questBook.Find("background/Buttons/Close").GetComponent<Button>();
        questBookCloseButton.onClick.AddListener(() => {
            questBook.gameObject.SetActive(false);
        });
        
        //Quest Info
        questInfoCompleteButton = questInfo.Find("background/Buttons/Complete").GetComponent<Button>();

        LevelUPText = canvas.Find("levelText");

        inventory = canvas.Find("Inventory");
        inventoryCloseButton = inventory.Find("background/Buttons/Close").GetComponent<Button>();

        inventoryCloseButton.onClick.AddListener(() => {
             inventory.gameObject.SetActive(false);
         });
    }

    public Vector2 WorldToCanvansPoint (Vector3 position)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);

        Vector2 CanvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

        return new Vector2(viewportPoint.x * CanvasSize.x, viewportPoint.y * CanvasSize.y) - (CanvasSize / 2);
    }

    public Vector2 ScreenToCanvansPoint(Vector3 screenPosition)
    {
        Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(screenPosition);

        Vector2 CanvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

        return new Vector2(viewportPoint.x * CanvasSize.x, viewportPoint.y * CanvasSize.y) - (CanvasSize / 2);
    }


    public void ToggleInventory() { 
        inventory.gameObject.SetActive(!inventory.gameObject.activeInHierarchy);
    }

}
