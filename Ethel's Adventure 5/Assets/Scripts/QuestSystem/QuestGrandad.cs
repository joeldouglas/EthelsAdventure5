using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        Sea,
        StartQuest,
        HandIn
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

    public static Dictionary<Quest, DialogueStarter> questDialogues = new();

    public static Dictionary<Quest, QuestInteraction> questHandIns = new();



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
                if (obj.interactableType == InteractableType.StartQuest ||
                    obj.interactableType == InteractableType.HandIn)
                        continue;

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
            {
                activeQuests[parentQuest] = true;

                // switch dialogue to post-quest
                DialogueStarter dlg = questDialogues[parentQuest];
                GetDialogueUpdate(out dlg.firstLineID, out dlg.lastLineID);

                // switch interaction to hand-in
                QuestInteraction qi = questHandIns[parentQuest];
                qi.interactableType = InteractableType.HandIn;

            }

        }


        private bool CheckQuestComplete()
        {            
            foreach (KeyValuePair<QuestObjective, bool> kvp in objectivesComplete)
                if (!kvp.Value) return false;
            return true;
        }


        private void GetDialogueUpdate(out int newStart, out int newEnd)
        {

            switch (parentQuest.questName)
            {
                case QuestName.Collect6Fish:
                    newStart = 8; newEnd = 9; break;

                case QuestName.BackAlleys1Fish:
                    newStart = 13; newEnd = 14; break;

                case QuestName.SearchForestAndSea:
                    newStart = 18; newEnd = 19; break;

                default:
                    newStart = 0; newEnd = 1; break;

            }

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
