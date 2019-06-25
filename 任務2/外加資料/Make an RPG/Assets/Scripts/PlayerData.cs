using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PlayerData {

    
    public static Dictionary<int, ActiveQuest> activeQuests = new Dictionary<int, ActiveQuest>();
    public static List<int> finishedQuests = new List<int>();

    public static Dictionary<int, MonsterKills> monstersKilled = new Dictionary<int, MonsterKills>();


    public static void AddQuest (int id)
    {
        if (activeQuests.ContainsKey(id)) return;

        Quest quest = QuestManager.instance.questsDictionary[id];

        ActiveQuest newActiveQuest = new ActiveQuest();
        newActiveQuest.id = id;
        newActiveQuest.dateTaken = DateTime.Now.ToLongDateString();

        if (quest.task.kills.Length > 0)
        {
            //set the kills of the new active quest as a new array of length of the kills in the quest
            newActiveQuest.kills = new Quest.QuestKill[quest.task.kills.Length];
            //for every kill in our quest.task, 
            foreach (Quest.QuestKill questKill in quest.task.kills)
            {
                //Set each quest kill to a new instance of questKill
                newActiveQuest.kills[questKill.id] = new Quest.QuestKill();
                //set the player current amount of kills of the new active quest based on the actual amount of monsters that player has killed
                if (!monstersKilled.ContainsKey(questKill.id)) monstersKilled.Add(questKill.id, new PlayerData.MonsterKills());

                newActiveQuest.kills[questKill.id].initialAmount = monstersKilled[questKill.id].amount;
            }
        }
        activeQuests.Add(id, newActiveQuest);
    }

    

    public class MonsterKills
    {
        public int id;
        public int amount;
    }

    public class ActiveQuest
    {
        public int id;  //Id of the quest taken.
        public string dateTaken;
        public Quest.QuestKill[] kills; //Holds the task monster ID and the amount of current kills when the quest was accepted.

    }
}
