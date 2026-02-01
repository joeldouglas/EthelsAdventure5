using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
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

}
