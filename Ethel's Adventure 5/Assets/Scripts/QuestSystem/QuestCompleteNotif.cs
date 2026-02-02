using UnityEngine;
using System.Collections;

using TMPro;

public class QuestCompleteNotif : MonoBehaviour
{

    public TMP_Text t;
    public float showForSeconds = 5f;


    private void Start()
    {
        //Complete(null);
    }


    public void Begin(Quest quest)
    {
        t.text = $"New Quest: {GetQuestObjective(quest)}";

        StartCoroutine(DestroyAfterDelay());

        LeanTween.value(25, 50, showForSeconds)
            .setEaseOutCubic()
            .setOnUpdate((float value) =>
            {
                t.characterSpacing = value;
            });
    }


    public void Complete(Quest quest)
    {

        //t.text = $"Quest Complete! Return to {quest.info.turnInQuestTo}";
        t.text = $"Quest Complete! Return to {quest.info.turnInQuestTo}";

        StartCoroutine(DestroyAfterDelay());

        LeanTween.value(25, 50, showForSeconds)
            .setEaseOutCubic()
            .setOnUpdate((float value) =>
            {
                t.characterSpacing = value;
            });

    }



    private string GetQuestObjective(Quest quest)
    {
        return quest.questName switch
        {
            QuestGrandad.QuestName.Collect6Fish => "Collect 6 Fish!",
            QuestGrandad.QuestName.BackAlleys1Fish => "Collect 1 Fish from the Back Alley!",
            QuestGrandad.QuestName.SearchForestAndSea => "Search the Forest and the Sea for Fish!",
            _ => "idk probably get some fish"
        };
    }



    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(showForSeconds);
        Destroy(this.gameObject);
    }

}
