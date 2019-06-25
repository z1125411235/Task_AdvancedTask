using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;


    public Transform canvas;
    //Quest Info
    public Transform questInfo;
    public Transform questInfoContent;
    public Button questInfoAcceptButton;
    public Button questInfoCancelButton;
    public Button questInfoCompleteButton;
    //Quest Book - Quest Gridcanvas
    public Transform questBook;
    public Transform questBookContent;
    public Button questBookCancelButton;

    public Transform levelUpText;

    //Inventory
    public Transform inventory;

    // Use this for initialization
    void Awake()
    {
        if (!instance) instance = this;
        canvas = GameObject.Find("Canvas").transform;

        //Quest Info
        questInfo = canvas.Find("Quest Info");
        questInfoContent = questInfo.Find("Background/Info/Viewport/Content");
        questInfoAcceptButton = questInfo.Find("Background/Buttons/Accept").GetComponent<Button>();
        questInfoCompleteButton = questInfo.Find("Background/Buttons/Complete").GetComponent<Button>();

        //Quest Info Cancel Button
        questInfoCancelButton = questInfo.Find("Background/Buttons/Cancel").GetComponent<Button>();
        questInfoCancelButton.onClick.AddListener(() => {
            StartCoroutine(ToggleOff(questInfo.gameObject));
        });

        //Quest Book
        questBook = canvas.Find("Quest Book");
        questBookContent = questBook.Find("Background/Info/Viewport/Content");

        //Quest Book Cancel Button
        questBookCancelButton = questBook.Find("Background/Buttons/Cancel").GetComponent<Button>();
        questBookCancelButton.onClick.AddListener(() => {
            StartCoroutine(ToggleOff(questInfo.gameObject));
        });

        //Level Up Text
        levelUpText = canvas.Find("LevelUp_Text");

        //Initialize Inventory
        inventory = canvas.Find("Inventory");

    }

    IEnumerator ToggleOff(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        go.SetActive(false);
    }

    public Vector2 WorldToCanvasPoint(Vector3 position)
    {
        //First get the position to viewport coordinates.
        //viewport point goes from 0,0 to 1,1 starting at bottom left
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);

        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

        return (new Vector2(viewportPoint.x * canvasSize.x, viewportPoint.y * canvasSize.y) - (canvasSize / 2));
    }



    public Vector2 ScreenToCanvasPoint(Vector2 screenPosition)
    {
        Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(screenPosition);

        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

        return (new Vector2(viewportPoint.x * canvasSize.x, viewportPoint.y * canvasSize.y) - (canvasSize / 2));
    }

    public void ToggleInventory()
    {
        inventory.gameObject.SetActive(!inventory.gameObject.activeInHierarchy);
    }



}
