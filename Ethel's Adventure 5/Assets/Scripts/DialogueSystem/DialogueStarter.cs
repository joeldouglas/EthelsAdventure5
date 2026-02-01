using UnityEngine;

using System.Collections;

public class DialogueStarter : MonoBehaviour
{

    public GameObject DialogueSystem;

    public int firstLineID;
    public int lastLineID;

    public void StartDialogue()
    {

        StartCoroutine(_start());
    }

    IEnumerator _start()
    {

        if (lib_DialogueLines.dialoguePool == null ||
            lib_DialogueLines.dialoguePool.Count == 0)
        {
            lib_DialogueLines.LoadDialogue();
            yield return null;
        }

        DialogueSystem ds = Instantiate(DialogueSystem).GetComponent<DialogueSystem>();
        ds.Init(firstLineID, lastLineID);

    }


}
