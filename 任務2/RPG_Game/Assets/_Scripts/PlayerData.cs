using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PlayerData
{

    //Use active quest class instead of only id.
    public static Dictionary<int, ActiveQuest> activeQuests = new Dictionary<int, ActiveQuest>();
    public static List<int> finishedQuests = new List<int>();
    //Dictionary holding the monsters ids and the number we have killed
    public static Dictionary<int, MonsterKills> monstersKilled = new Dictionary<int, MonsterKills>();

    /// <summary>
    /// Adds the quest to "activeQuests" List
    /// </summary>
    /// <param name="id">Identifier.</param>
    public static void AddQuest(int id)
    {
        //If we already accepted this quest, we won't accept it again.
        if (activeQuests.ContainsKey(id)) return;

        //Otherwise, we create a new ActiveQuest.
        Quest quest = QuestManager.instance.questDictionary[id];    //Set a reference to the quest we want to add.
        ActiveQuest newActiveQuest = new ActiveQuest();
        newActiveQuest.id = id;
        newActiveQuest.dateTaken = DateTime.Now.ToLongDateString();
        //If we need to kill monsters on this quest...
        if (quest.task.kills.Length > 0)
        {
            //set the kills of the new active quest as a new array of length of the kills in the quest
            newActiveQuest.kills = new Quest.QuestKill[quest.task.kills.Length];
            //for every kill in our quest.task, 
            int killIndex = 0;
            foreach (Quest.QuestKill questKill in quest.task.kills)
            {
                //Set each quest kill to a new instance of questKill
                newActiveQuest.kills[killIndex] = new Quest.QuestKill();
                //set the player current amount of kills of the new active quest based on the actual amount of monsters that player has killed
                if (!monstersKilled.ContainsKey(questKill.id)) monstersKilled.Add(questKill.id, new PlayerData.MonsterKills());

                newActiveQuest.kills[killIndex].initialAmount = monstersKilled[questKill.id].amount;
                killIndex++;
            }
        }
        activeQuests.Add(id, newActiveQuest);
    }

    //How many monsters[id] have we killed in total.
    public class MonsterKills
    {
        public int id;
        public int amount;
    }

    //Holds information specific to the instance of this quest. Useful for repeatable quests and counting the number of mosters killed
    public class ActiveQuest
    {
        public int id;  //Id of the quest taken.
        public string dateTaken;
        public Quest.QuestKill[] kills; //Holds the task monster ID and the amount of current kills when the quest was accepted.

    }
}