using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour, IClickable
{

    [SerializeField] int[] quests;
    [SerializeField] string[] dialogues;
    public int dialogueIndex = 0;

    void Start()
    {
        foreach (int i in quests)
        {
            QuestManager.instance.LoadQuest(i);
        }
    }

    public void ShowDialogue()
    {
        if (dialogueIndex > (dialogues.Length - 1))
        {
            DialogueManager.instance.CloseDialogueBox();
            dialogueIndex = (dialogues.Length - 2);
        }
        else
        {
            DialogueManager.instance.PrintOnDialogueBox(name + ": " + dialogues[dialogueIndex]);
        }
    }

    //This should be done in a separate
    public void ShowQuestInfo()
    {
        foreach (int i in quests)
        {
            if (
                //Did the player Finished this quest?
                !PlayerData.finishedQuests.Contains(i) &&
                //Do the player meet the requirements?
                QuestManager.instance.IsQuestAvailable(i, GameObject.Find("Player").GetComponent<PlayerController>())
            )
            {
                //Show the info of the quest
                QuestManager.instance.ShowQuestInfo(QuestManager.instance.questDictionary[quests[i]]);

                //Show/Hide accept Button depending on "do we have this quest?"
                UIManager.instance.questInfoAcceptButton.gameObject.SetActive(!PlayerData.activeQuests.ContainsKey(i));

                //Set the Complete Quest Button
                if (QuestManager.instance.IsQuestFinished(i))
                {
                    UIManager.instance.questInfoCompleteButton.gameObject.SetActive(true);
                    UIManager.instance.questInfoCompleteButton.onClick.AddListener(() => {
                        ReceiveCompletedQuest(QuestManager.instance.questDictionary[quests[i]]);
                        PlayerData.activeQuests.Remove(i);
                        PlayerData.finishedQuests.Add(i);
                        UIManager.instance.questInfoCompleteButton.onClick.RemoveAllListeners();
                        UIManager.instance.questInfo.gameObject.SetActive(false);
                    });
                }

                break;
            }
        }
    }

    void ReceiveCompletedQuest(Quest quest)
    {
        if (quest.reward.exp > 0) PlayerController.main.SetExperience(quest.reward.exp);
        if (quest.reward.items.Length > 0)
        {
            foreach (var item in quest.reward.items)
            {
                print("You get:(" + item.amount + ")x" + ItemDatabase.items[item.id]);
                //ex. inventory.add(item.id, item.amount);
            }
        }
    }

    public void OnLeftClick()
    {
        ShowQuestInfo();
    }

    public void OnRightClickDown()
    {
        throw new System.NotImplementedException();
    }
}


//1. Make main player
//2. Make ReceiveCompletedQuest(quest)
//3. Set Complete Button
//4. Tidy Up