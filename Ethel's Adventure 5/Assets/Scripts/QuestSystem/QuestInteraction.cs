using UnityEngine;
using System.Collections;

using static QuestGrandad;

public class QuestInteraction : MonoBehaviour
{

    public QuestName questName;
    public InteractableType interactableType;

    public GameObject questCompleteNotif;
    public GameObject questPrize;

    public void Interact()
    {
        Quest quest = questRefs[questName];

        // if starting quest...
        if (interactableType == InteractableType.StartQuest)
        {

            // prevent multiple instances of same quest
            if (activeQuests.ContainsKey(quest))
                return;

            // show quest begin notif
            Instantiate(questCompleteNotif)
                .GetComponent<QuestCompleteNotif>()
                .Begin(quest);

            // log in active quests
            activeQuests.Add(quest, false);

            // set this interaction component as the hand-in interaction for later
            questHandIns.Add(quest, this);

            return;

        }



        // if handing-in quest...
        else if (interactableType == InteractableType.HandIn)
        {
            if (activeQuests[quest])
                Instantiate(questPrize);
            return;
        }



        // else...

        // logs progress of objective
        quest.UpdateProgress(interactableType);
        
        // if the target objective count is met, displays notif 
        //      directing player to hand-in location
        //          (set in the QuestInfo struct in the Quest scriptable object)
        StartCoroutine(CheckComplete(quest));        

    }



    IEnumerator CheckComplete(Quest quest)
    {
        yield return null;

        if (activeQuests[quest])
        {
            Instantiate(questCompleteNotif)
                .GetComponent<QuestCompleteNotif>()
                .Complete(quest);            
        }

    }

}
