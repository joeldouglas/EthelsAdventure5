using UnityEngine;
using System.Collections.Generic;

public static class lib_DialogueLines
{

    public struct DialogueLine
    {
        public int id;
        public string speaker;
        public string line;

        public DialogueLine(int id, string speaker, string line)
        {
            this.id = id;
            this.speaker = speaker;
            this.line = line;
        }
            

    }

    public static Dictionary<int, DialogueLine> dialoguePool = new();


    public static void LoadDialogue()
    {

        Debug.Log("Loading dialogue");

        TextAsset csv = Resources.Load<TextAsset>("Ethel5 Dialogue");
        List<DialogueLine> fullList = DialogueCsvParser.LoadFromCsvText(csv.text);

        Debug.Log($"Line count = {fullList.Count}");

        foreach (DialogueLine line in fullList)
        {
            dialoguePool[line.id] = line;
            Debug.Log($"Adding Line:\n" +
                $"[{line.id}] ({line.speaker}) {line.line}");


        }

    }

}
