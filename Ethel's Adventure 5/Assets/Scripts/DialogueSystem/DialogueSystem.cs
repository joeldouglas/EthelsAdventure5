using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections;
using System.Collections.Generic;

using static lib_DialogueLines;
using static lib_CharacterSprites;
using UnityEngine.InputSystem;

[System.Serializable]
public class DialogueSystem : MonoBehaviour
{

    public GameObject root;

    public float dialogueSpeed = 0.2f;
    public Queue<DialogueLine> dialogueQueue = new();
    private string lastSpeaker = "";
    private bool isLeft = true;

    private bool canProgress = false;
    public GameObject progressPrompt;
    private bool skipped = false;

    [Header("Left")]
    public GameObject LEFT;
    public GameObject LEFT_T;
    public Image img_Left;    
    public TMP_Text t_LeftName;
    public TMP_Text t_LeftLine;

    [Header("Right")]
    public GameObject RIGHT;
    public GameObject RIGHT_T;
    public Image img_Right;
    public TMP_Text t_RightName;
    public TMP_Text t_RightLine;


    [Header("Background")]
    public CanvasGroup bg_Opacity;

    private void Awake()
    {

        
        
    }

    private void Start()
    {
        root = gameObject;

        if (root.activeSelf)
            root.SetActive(false);

        StartDialogue();
    }


    public void Init(int start, int end)
    {
        for (int i = start; i < end + 1; i++)
            dialogueQueue.Enqueue(dialoguePool[i]);

        StartDialogue();
    }


    public void StartDialogue()
    {                       
        /*
        Debug.Log($"Starting dialogue");

        DialogueLine dl1 = 
            new DialogueLine(0, "BillyBob", "pee pee poo poo wee wee");
        DialogueLine dl2 =
            new DialogueLine(1, "BobbyBill", "wa wa wee wee wa");

        dialogueQueue.Enqueue(dl1);
        dialogueQueue.Enqueue(dl2);
        */
        
        SpeakNextLine();

    }




    private void SpeakNextLine()
    {

        if (dialogueQueue.Count == 0)
        {
            Destroy(root);
            return;
        }



        DialogueLine dl = dialogueQueue.Dequeue();

        /*
        isLeft = string.IsNullOrEmpty(lastSpeaker) 
            ? Random.Range(0,2) == 0
            : lastSpeaker == dl.speaker ? isLeft : !isLeft;
        */
            
        bool newSpeaker = string.IsNullOrEmpty(lastSpeaker)
            ? false
            : lastSpeaker != dl.speaker;

        isLeft = newSpeaker ? !isLeft : isLeft;

        GameObject side = isLeft ? LEFT : RIGHT;
        Image img = isLeft ? img_Left : img_Right;
        TMP_Text speaker = isLeft ? t_LeftName : t_RightName;
        TMP_Text dialogue = isLeft ? t_LeftLine : t_RightLine;

        /* disabled for testing
        img.sprite = libSprites[dl.speaker];*/
        speaker.text = dl.speaker;
        dialogue.text = dl.line;
        lastSpeaker = dl.speaker;
        

        LEFT.SetActive(isLeft);
        RIGHT.SetActive(!isLeft);

        if (!root.activeSelf)
            root.SetActive(true);

        StartCoroutine(AnimateLine(
            isLeft, side, img.gameObject, speaker, dialogue
            ));


    }


    IEnumerator AnimateLine(bool isLeft, GameObject side, GameObject img, TMP_Text name, TMP_Text line)
    {        

        // hide prompt
        progressPrompt.SetActive(false);

        // cache text
        string text = line.text;

        // clear display
        line.maxVisibleCharacters = 0;
        line.text = string.Empty;
        yield return null;

        // reassign
        line.text = text;
        line.ForceMeshUpdate();

        // animate
        yield return StartCoroutine(Swoop(isLeft, img));

        int length = line.textInfo.characterCount;
        for (int i = 0; i < length; i++)
        {
            line.maxVisibleCharacters++;
            yield return new WaitForSeconds(dialogueSpeed);

            if (skipped)
            {
                line.maxVisibleCharacters = length;
                yield return null;
                skipped = false;
                break;
            }
        }

        canProgress = true;
        progressPrompt.SetActive(true);

    }


    private IEnumerator Swoop(bool isLeft, GameObject img)
    {

        GameObject TEXT = isLeft ? LEFT_T : RIGHT_T;

        // hide bg
        bg_Opacity.alpha = 0;

        // set bg rotation
        float rot = isLeft ? -180 : 0;
        bg_Opacity.transform.rotation = Quaternion.Euler(0, 0, rot);

        // position sprite
        float pos = isLeft ? 800 : -800;
        RectTransform i_rt = img.GetComponent<RectTransform>();
        i_rt.anchoredPosition = new Vector3(pos, 0, 0);
        float target = isLeft ? 200 : -200;

        // position text
             
        RectTransform t_rt = TEXT.GetComponent<RectTransform>();
        Vector3 t_pos = t_rt.anchoredPosition;
        float textY_up = t_rt.anchoredPosition.y;
        float textY_down = textY_up - 150f;
        t_rt.anchoredPosition = new Vector3(t_pos.x, textY_down, t_pos.z);

        // swoop guy in
        LeanTween.moveLocalX(img.gameObject, target, 0.75f)
            .setEaseOutExpo();

        // swoop text up
        LeanTween.moveLocalY(TEXT, textY_up, 0.5f)
            .setDelay(0.25f);                        

        // fade up bg
        LeanTween.alphaCanvas(bg_Opacity, 1, 0.75f)
            .setDelay(0.25f);

        
        yield return new WaitForSeconds(1);

        //StartCoroutine(WaitNext());

    }

    IEnumerator WaitNext()
    {
        yield return new WaitForSeconds(3);
        SpeakNextLine();
    }




    public void Progress()
    {
        SpeakNextLine();
    }


    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            if (!canProgress) skipped = true;
            else Progress();
    }

}
