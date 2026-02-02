using UnityEngine;
using System.Collections;

public class DialogueTester : MonoBehaviour
{

    public GameObject Dialogue;

    private void Start()
    {
        StartCoroutine(load() );       
    }

    IEnumerator load()
    {
        lib_DialogueLines.LoadDialogue();
        yield return null;
        GameObject obj = Instantiate(Dialogue);
        obj.GetComponent<DialogueSystem>().Init(0, 5, null);
    }

}
