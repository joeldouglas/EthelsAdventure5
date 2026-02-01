using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public static class QuestGrandad
{

    #region Enums

    public enum QuestName
    {
        Collect6Fish,
        BackAlleys1Fish,
        SearchForestAndSea
    }

    public enum InteractableType
    {
        Fish,
        Tree,
        Sea
    }

    #endregion



    #region Structs

    [System.Serializable]
    public struct QuestInfo
    {             
        public string turnInQuestTo;
        public List<QuestObjective> objectives;
    }

    [System.Serializable]
    public struct QuestObjective
    {
        public InteractableType interactableType;
        public int requiredInteractions;
    }

    #endregion


    public static Dictionary<QuestName, Quest> questRefs = new();

    public static Dictionary<Quest, bool> activeQuests = new();

    public static Dictionary<Quest, QuestProgress> questProgresses = new();



    #region Loading New Quests

    // THIS WILL BE CREATED WHEN PLAYER TAKES QUEST
    public class QuestProgress
    {

        public Quest parentQuest;
        public Dictionary<InteractableType, int> objectivesProgress = new();
        public Dictionary<InteractableType, QuestObjective> objectiveRefs = new();
        public Dictionary<QuestObjective, bool> objectivesComplete = new(); 

        public QuestProgress(Quest quest)
        {

            if (questProgresses.ContainsKey(quest))
            {
                Debug.LogError($"Player already has Quest of {quest.questName}");
                return;
            }
            else
            {
                questRefs[quest.questName] = quest;
                activeQuests[quest] = false;
                questProgresses[quest] = this;
            }

            parentQuest = quest;
            QuestInfo info = quest.info;
            foreach (QuestObjective obj in info.objectives)
            {                
                objectivesProgress[obj.interactableType] = 0;
                objectiveRefs[obj.interactableType] = obj;
                objectivesComplete[obj] = false;
            }

        }


        public void UpdateProgress(QuestName name, InteractableType intType)
        {
            QuestProgress progress = questRefs[name].GetProgress();

            progress.objectivesProgress[intType]++;
            int currentProgress = progress.objectivesProgress[intType];

            QuestObjective obj = progress.objectiveRefs[intType];

            if (isObjectiveComplete(obj, currentProgress))
                CompleteObjective(obj);

            // then check if quest is complete

        }


        private bool isObjectiveComplete(QuestObjective obj, int currentProgress)
        {
            return (currentProgress == obj.requiredInteractions);
        }


        public void CompleteObjective(QuestObjective obj)
        {
            objectivesComplete[obj] = true;

            if (CheckQuestComplete())
                QuestGrandad.activeQuests[parentQuest] = true;

        }


        private bool CheckQuestComplete()
        {            
            foreach (KeyValuePair<QuestObjective, bool> kvp in objectivesComplete)
                if (!kvp.Value) return false;
            return true;
        }



    }

    #endregion



    #region Quest Progress

    
    public static bool IsQuestComplete(Quest quest)
    {
        return activeQuests[quest];
    }


    #endregion


}
