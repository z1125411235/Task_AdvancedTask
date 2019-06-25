using System.Collections;
using UnityEngine;
using System;

public class Quest
{

    public int id;
    public string questName;
    public string description;
    public int recipent;        //ID of NPC who recieves the quest once finished.
    public int requiredLevel;
    public Reward reward;
    public Task task;

    [Serializable]
    public class Reward
    {
        public float exp;
        public float money;
        public QuestItem[] items;

    }
    [Serializable]
    public class Task
    {
        public int[] talkTo;            //id of the NPC(s) who we have to talk to to complete the quest
        public QuestItem[] items;
        public QuestKill[] kills;
    };

    [Serializable]
    public class QuestItem
    {
        public int id;
        public int amount;
    }

    [Serializable]
    public class QuestKill
    {
        public int id;
        public int amount;
        public int initialAmount;
    }
}
