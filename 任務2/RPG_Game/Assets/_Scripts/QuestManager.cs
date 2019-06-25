using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{

    public static QuestManager instance;
    public Dictionary<int, Quest> questDictionary = new Dictionary<int, Quest>();


    void Awake()
    {
        if (instance == null) instance = this;
        SetCallbacks();
    }

    /// <summary>
    /// Loads the quest from Json and stores it in the Quest Dictionary as <Quest>.
    /// </summary>
    /// <param name="id">Identifier.</param>
    public void LoadQuest(int id)
    {
        Quest newQuest = JsonUtility.FromJson<Quest>(Resources.Load<TextAsset>("Json Files/" + id.ToString("00")).text);
        questDictionary.Add(newQuest.id, newQuest);
    }

    void SetCallbacks()
    {
        InputManager.KeyPressDown += KeyCallbacks;
    }

    void KeyCallbacks()
    {
        if (Input.inputString == "b")
        {
            ToggleQuestBook(!UIManager.instance.questBook.gameObject.activeInHierarchy);
        }
    }

    void ToggleQuestBook(bool b)
    {
        UIManager.instance.questBook.gameObject.SetActive(b);
        if (b) ShowActiveQuests();
    }

    public void ShowActiveQuests()
    {
        foreach (PlayerData.ActiveQuest activeQuest in PlayerData.activeQuests.Values)
        {
            int i = activeQuest.id;

            if (UIManager.instance.questBookContent.Find(i.ToString()) != null)
            {
                continue;   //If we found this quest id as one of the children of questBookContent, we skip the creation of this button
            }

            //Create New Quest Button
            GameObject QuestButtonGo = Instantiate(Resources.Load("Prefabs/Quest_Button_Prefab") as GameObject);
            QuestButtonGo.name = questDictionary[i].id.ToString();
            QuestButtonGo.transform.SetParent(UIManager.instance.questBookContent);
            QuestButtonGo.transform.localScale = Vector3.one;
            QuestButtonGo.transform.Find("Text").GetComponent<Text>().text = questDictionary[i].questName;
            int questId = new int();
            questId = i;
            QuestButtonGo.GetComponent<Button>().onClick.AddListener(() => {
                ShowQuestInfo(questDictionary[questId]);
            });
        }
    }

    public void ShowQuestInfo(Quest quest)
    {
        //Show Quest Info Panel
        UIManager.instance.questInfo.gameObject.SetActive(true);


        //Hide the complete button. It will be opened by the NPC if appropriate
        UIManager.instance.questInfoCompleteButton.gameObject.SetActive(false);

        //Remove previous functions from the ACCEPT button
        UIManager.instance.questInfoAcceptButton.onClick.RemoveAllListeners();
        //Set function on ACCEPT Button
        UIManager.instance.questInfoAcceptButton.onClick.AddListener(() => {
            PlayerData.AddQuest(quest.id);
            UIManager.instance.questInfo.gameObject.SetActive(false);
            ShowActiveQuests();
        });
        //Set Texts
        UIManager.instance.questInfoContent.Find("Name").GetComponent<Text>().text = quest.questName;
        UIManager.instance.questInfoContent.Find("Description").GetComponent<Text>().text = quest.description;
        //TASK
        string taskString = "Task:\n";
        if (quest.task.kills != null)
        {
            int killIndex = 0;
            foreach (Quest.QuestKill qk in quest.task.kills)
            {
                //Current kills is zero when we haven't taken the quest.
                int currentKills = 0;
                if (PlayerData.activeQuests.ContainsKey(quest.id) && PlayerData.monstersKilled.ContainsKey(qk.id))
                    //if we are showing the info during the progress of the quest (we took it already) show the progress.
                    currentKills = PlayerData.monstersKilled[qk.id].amount - PlayerData.activeQuests[quest.id].kills[killIndex].initialAmount;
                taskString += "Slay " + (currentKills) + "/" + qk.amount + " " + MonsterDatabase.monsters[qk.id] + ".\n";
                killIndex++;
            }
        }
        if (quest.task.items != null)
        {
            foreach (Quest.QuestItem qi in quest.task.items)
            {
                taskString += "Bring " + qi.amount + " " + ItemDatabase.items[qi.id] + ".\n";
            }
        }
        if (quest.task.talkTo != null)
        {
            foreach (int id in quest.task.talkTo)
            {
                taskString += "Talk To " + NPCDatabase.npcs[id] + ".\n";
            }
        }
        UIManager.instance.questInfoContent.Find("Task").GetComponent<Text>().text = taskString;
        //REWARD
        string rewardString = "Reward:\n";
        if (quest.reward.items != null)
        {
            foreach (Quest.QuestItem qi in quest.reward.items)
            {
                rewardString += qi.amount + " " + ItemDatabase.items[qi.id] + ".\n";
            }
        }
        if (quest.reward.exp > 0) rewardString += quest.reward.exp + " Experience.\n";
        if (quest.reward.money > 0) rewardString += quest.reward.money + " Money.\n";
        UIManager.instance.questInfoContent.Find("Reward").GetComponent<Text>().text = rewardString;

        //Content Fitter is a bit Buggy, it won't reset the size after text is changed.
        StartCoroutine(RestartContentFitter());
    }

    //Quick Fix for Content Fitter Bug
    IEnumerator RestartContentFitter()
    {
        UIManager.instance.questInfoContent.GetComponent<ContentSizeFitter>().enabled = false;
        yield return new WaitForEndOfFrame();
        UIManager.instance.questInfoContent.GetComponent<ContentSizeFitter>().enabled = true;
    }

    //Check if the player meet the requirements for being offered the quest
    public bool IsQuestAvailable(int questId, PlayerController player)
    {
        return (questDictionary[questId].requiredLevel <= player.level);
    }

    public bool IsQuestFinished(int questId)
    {
        Quest quest = questDictionary[questId];
        //Check Kills
        //If there is at least one kill that we are required to do.
        if (quest.task.kills.Length > 0)
        {
            //Foreach kill that we must do
            int killIndex = 0;
            foreach (var questKill in quest.task.kills)
            {
                if (!PlayerData.monstersKilled.ContainsKey(questKill.id) || !PlayerData.activeQuests.ContainsKey(quest.id))
                {
                    return false;
                }
                int currentKills = PlayerData.monstersKilled[questKill.id].amount - PlayerData.activeQuests[quest.id].kills[killIndex].initialAmount;
                if (currentKills < questKill.amount)
                {
                    return false;
                }
            }
        }
        //Do the same but check Items on Inventory.
        //If we don't have the required items at any point, return false.

        //Same for "talked to". Return false if incomplete.

        //If at any point the quest is incomplete, we would have returned false and stop running.
        //Since we reach this point, the quest is complete, so we return true.
        return true;
    }

}


//Make Input Manager
//Close and open quest book
//Suggested, change naming : QuestKill.playercurrent -> initialAmount
//Make IsQuestCompleted()
//Make Complete Button on UI Manager

//Turn on that button if the NPC checked the quest is completed
