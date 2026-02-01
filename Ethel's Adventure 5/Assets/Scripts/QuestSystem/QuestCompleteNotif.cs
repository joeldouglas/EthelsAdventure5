using UnityEngine;
using System.Collections;

using TMPro;

public class QuestCompleteNotif : MonoBehaviour
{

    public TMP_Text t;
    public float showForSeconds = 5f;


    private void Start()
    {
        Init(null);
    }

    public void Init(Quest quest)
    {

        //t.text = $"Quest Complete! Return to {quest.info.turnInQuestTo}";
        t.text = "Quest Complete! Return to the Casino";

        StartCoroutine(DestroyAfterDelay());

        LeanTween.value(25, 50, showForSeconds)
            .setEaseOutCubic()
            .setOnUpdate((float value) =>
            {
                t.characterSpacing = value;
            });

    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(showForSeconds);
        Destroy(this.gameObject);
    }

}
