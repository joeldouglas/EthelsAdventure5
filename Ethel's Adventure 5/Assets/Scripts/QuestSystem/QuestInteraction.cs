using UnityEngine;
using System.Collections;

using static QuestGrandad;

public class QuestInteraction : MonoBehaviour
{

    public QuestName questName;
    public InteractableType interactableType;

    public GameObject questCompleteNotif;

    public void Interact()
    {
        Quest quest = questRefs[questName];
        quest.UpdateProgress(interactableType);

        StartCoroutine(CheckComplete(quest));

    }

    IEnumerator CheckComplete(Quest quest)
    {
        yield return null;

        if (activeQuests[quest])
        {
            Instantiate(questCompleteNotif)
                .GetComponent<QuestCompleteNotif>()
                .Init(quest);            
        }

    }

}
