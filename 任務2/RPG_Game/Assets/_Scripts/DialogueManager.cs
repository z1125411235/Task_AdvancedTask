using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager instance;

    private Transform dialogueBox;
    private Text dialogueBoxTextBody;

    // Use this for initialization
    void Start()
    {
        if (instance == null) instance = this;

        dialogueBox = UIManager.instance.canvas.Find("Dialogue Box");
        dialogueBoxTextBody = dialogueBox.Find("Background/Text").GetComponent<Text>();
    }

    public void PrintOnDialogueBox(string text)
    {
        dialogueBox.gameObject.SetActive(true);
        dialogueBoxTextBody.text = text;
        InputManager.OnPressUp += CloseDialogueBoxCallback;
    }

    public void CloseDialogueBox()
    {
        dialogueBox.gameObject.SetActive(false);
    }

    public void CloseDialogueBoxCallback()
    {
        CloseDialogueBox();
        InputManager.OnPressUp -= CloseDialogueBoxCallback;
    }

}
