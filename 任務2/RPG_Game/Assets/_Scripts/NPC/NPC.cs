using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC class will talk to player when clicked.
/// for more behaviour, please extend this class.
/// </summary>

public class NPC : MonoBehaviour, IClickable
{

    public string Dialogue;
    public System.Action OnTalk;

    public void OnLeftClick()
    {
        Talk();
    }

    void Talk()
    {
        DialogueManager.instance.PrintOnDialogueBox(Dialogue);
        if (OnTalk != null) OnTalk();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnRightClickDown()
    {
        throw new System.NotImplementedException();
    }
}
