using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {

    public static QuestManager instance;

    public Dictionary<int, Quest> questsDictionary = new Dictionary<int, Quest>();

    private void Awake()
    {
        if (instance == null) instance = this;
        SetCallbacks();
    }

    private void Update()
    {
        foreach (var p in PlayerData.activeQuests)
        {
            Debug.Log(p.Key);
        }
    }

 
    public void LoadQuestInforfromJson(int id)
    {
        StreamReader file = new StreamReader(System.IO.Path.Combine(Application.streamingAssetsPath, id.ToString("00") + ".json"));
        // Assets/streamingAssets

        Debug.Log(id.ToString("#' jfile'"));
        string loadString = file.ReadToEnd();
        file.Close();

        Quest newQuest = JsonUtility.FromJson<Quest>(loadString);

        if (!questsDictionary.ContainsKey(0))
        questsDictionary.Add(newQuest.id, newQuest);
    }

    void SetCallbacks()
    {
        InputManager.KeyPressDown += KeyCallbacks;
    }

    void KeyCallbacks()
    {
        if (Input.inputString == "b" || Input.inputString == "B")
        {
            ToggleQuestBook(!(UIManager.instance.questBook.gameObject.activeInHierarchy));
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
            GameObject QuestButtonGo = Instantiate(Resources.Load("Prefabs/QuestButtonPrefab") as GameObject);
            QuestButtonGo.name = questsDictionary[i].id.ToString();
            QuestButtonGo.transform.SetParent(UIManager.instance.questBookContent);
            QuestButtonGo.transform.localScale = Vector3.one;
            QuestButtonGo.transform.Find("Text").GetComponent<Text>().text = questsDictionary[i].questName;
            UIManager.instance.questInfoAcceptButton.gameObject.SetActive(false);
            int questId = new int();
            questId = i;
            QuestButtonGo.GetComponent<Button>().onClick.AddListener(() => {
                ShowQuestInfo(questsDictionary[questId]);
            });
        }
    }

    public void ShowQuestInfo(Quest quest)
    {
        //Show Quest Info Panel
        UIManager.instance.questInfo.gameObject.SetActive(true);
        //Show/Hide accept Button depending on "do we have this quest?".
       
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
        UIManager.instance.questInfo.Find("background/Name").GetComponent<Text>().text = quest.questName;
        UIManager.instance.questInfoContent.Find("Description").GetComponent<Text>().text = quest.description;
        //TASK

        if (PlayerData.finishedQuests.Contains(quest.id))
        {
            UIManager.instance.questInfoContent.Find("Task").GetComponent<Text>().text = "QuestComplete. ";
            UIManager.instance.questInfoContent.Find("Reward").GetComponent<Text>().text = "";
            return;
        }

        string taskString = "Task:\n";
        if (quest.task.kills != null)
        {
            foreach (Quest.QuestKill qk in quest.task.kills)
            {
                //Current kills is zero when we haven't taken the quest.
                int currentKills = 0;
                if (PlayerData.activeQuests.ContainsKey(quest.id) && PlayerData.monstersKilled.ContainsKey(qk.id)) {
                    currentKills = PlayerData.monstersKilled[qk.id].amount - PlayerData.activeQuests[quest.id].kills[qk.id].initialAmount;
                }
                    //if we are showing the info during the progress of the quest (we took it already) show the progress.
                 
                taskString += "Slay " + (currentKills) + "/" + qk.amount + " " + MonsterDatabase.monsters[qk.id] + ".\n";
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
                taskString += "Talk To " + NPCDatabase.NPCs[id] + ".\n";
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
        return (questsDictionary[questId].requiredLevel <= player.level);
    }

    public bool IsQuestFinished(int questId)
    {
        Quest quest = questsDictionary[questId];
        //Check Kills
        //If there is at least one kill that we are required to do.
        if (quest.task.kills.Length > 0)
        {
            //Foreach kill that we must do
            foreach (var questKill in quest.task.kills)
            {
                if (!PlayerData.monstersKilled.ContainsKey(questKill.id) || !PlayerData.activeQuests.ContainsKey(quest.id))
                {
                    return false;
                }
                int currentKills = PlayerData.monstersKilled[questKill.id].amount - PlayerData.activeQuests[quest.id].kills[questKill.id].initialAmount;
                if (currentKills < questKill.amount)
                {
                    return false;
                }
            }
        }
        return true;
    }

}

