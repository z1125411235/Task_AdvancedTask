using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager instance;

    public Transform dialogueBox;
    private Text dialogueBoxNameTextBody;
    private Text dialogueBoxContentTextBody;
    

    private void Awake()
    {
       
      
    }
    // Use this for initialization
    void Start () {
        if (instance == null) instance = this;
        dialogueBox = UIManager.instance.canvas.Find("dialogueBox");
        dialogueBoxContentTextBody = dialogueBox.Find("contentText").GetComponent<Text>();
        dialogueBoxNameTextBody = dialogueBox.Find("NameText").GetComponent<Text>();

    }


    /* public void PrintOnDialogueBox()
     {
         OpenDialogueBox();

         dialogueBoxContentTextBody.text = text;
         dialogueBoxNameTextBody.text = name;
         InputManager.OnPressUp += CloseDialogueBoxCallback;
     }*/

    public void PrintOnDialogueBox(string text, string name)
    {
        dialogueBox.gameObject.SetActive(true);
        dialogueBoxContentTextBody.text = text;
        dialogueBoxNameTextBody.text = name;
        InputManager.OnPressUp += CloseDialogueBoxCallback;
    }

    public void CloseDialogueBox()
    {
        dialogueBox.gameObject.SetActive(false);
    }

    public void CloseDialogueBoxCallback()
    {
        InputManager.OnPressUp -= CloseDialogueBoxCallback;
        StartCoroutine(DialoguekRoutine());        
        //CloseDialogueBox();
    }

    IEnumerator DialoguekRoutine()
    {
        yield return new WaitForSeconds(2);
        CloseDialogueBox();

    }

}
