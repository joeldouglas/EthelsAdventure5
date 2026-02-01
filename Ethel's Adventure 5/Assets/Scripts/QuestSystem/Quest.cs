using UnityEngine;

using static QuestGrandad;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{

    public QuestName questName;
    [SerializeField] public QuestInfo info;

    private QuestProgress progress;


    // CALL THIS FROM WHERE-EVER THE QUEST IS INITIATED
    public void StartQuest()
    {
        progress = new QuestProgress(this);
    }

    public void UpdateProgress(InteractableType type)
    {
        progress.UpdateProgress(questName, type);
    }

    public QuestProgress GetProgress()
    {
        return progress;
    }

}
