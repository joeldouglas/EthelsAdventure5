using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro
using UnityEngine.UI; // Required for Image

public class GachaBehaviour : MonoBehaviour
{
    [Header("Gacha Library")]
    [SerializeField] private List<Mask> maskLibrary; // Drag your Archetype ScriptableObjects here

    [Header("UI References")]
    [SerializeField] private GameObject prizePanel;
    [SerializeField] private TextMeshProUGUI prizeText;
    [SerializeField] private Image prizeImage;
    // This variable should hold the mask we just won
    private Mask currentPendingMask; 


    [Header("Settings")]
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private float spinDuration = 5f;

    [Header("Machine References")]
    [SerializeField] private GameObject handleObject;
    [SerializeField] private SpriteRenderer[] wheelSprites;

    private bool isSpinning = false;
    private bool canSpin = true;

    // --- BUTTON TRIGGER ---
    public void SpinSlotMachine()
    {
        if (isSpinning || !isEnabled || !canSpin)
        {
            Debug.Log("<color=yellow>GACHA:</color> Machine is busy or disabled.");
            return;
        }

        isSpinning = true;
        canSpin = false;
        prizePanel.SetActive(false); // Hide previous prize if open

        StartCoroutine(SquashHandleEffect());
        StartCoroutine(WheelSpinEffect());
    }

    // --- HANDLE ANIMATION ---
    private IEnumerator SquashHandleEffect()
    {
        if (handleObject == null) yield break;

        Vector3 fullScale = Vector3.one;
        Vector3 squashedScale = new Vector3(1, 0.5f, 1);
        float halfTime = 0.25f;
        float elapsed = 0f;

        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            handleObject.transform.localScale = Vector3.Lerp(fullScale, squashedScale, elapsed / halfTime);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            handleObject.transform.localScale = Vector3.Lerp(squashedScale, fullScale, elapsed / halfTime);
            yield return null;
        }
        handleObject.transform.localScale = fullScale;
    }

    // --- WHEEL ANIMATION ---
    private IEnumerator WheelSpinEffect()
    {
        Debug.Log("<color=cyan>GACHA:</color> Wheels spinning...");
        float timeTracker = 0f;
        float colorTicket = 0f;

        while (timeTracker < spinDuration)
        {
            timeTracker += Time.deltaTime;
            float progress = timeTracker / spinDuration;
            float currentFlashDelay = 0.05f;

            if (progress > 0.8f) // Slow down last 20%
            {
                currentFlashDelay = Mathf.Lerp(0.05f, 0.6f, (progress - 0.8f) / 0.2f);
            }

            colorTicket += Time.deltaTime;
            if (colorTicket >= currentFlashDelay)
            {
                foreach (SpriteRenderer wheel in wheelSprites)
                {
                    if (wheel != null) 
                        wheel.color = new Color(Random.value, Random.value, Random.value);
                }
                colorTicket = 0f;
            }
            yield return null;
        }

        foreach (SpriteRenderer wheel in wheelSprites)
        {
            if (wheel != null) wheel.color = Color.white;
        }

        // --- ROLL FOR PRIZE ---
        float spinResult = Random.Range(0f, 1f);
        int tierIndex = 0;
        if (spinResult > 0.95f) tierIndex = 4;      // Legendary
        else if (spinResult > 0.8f) tierIndex = 3;  // Very Rare
        else if (spinResult > 0.6f) tierIndex = 2;  // Rare
        else if (spinResult > 0.3f) tierIndex = 1;  // Uncommon
        else tierIndex = 0;                         // Common

        StartCoroutine(ShowPrizeSequence(tierIndex));
    }

    // --- UI DISPLAY ---
    private IEnumerator ShowPrizeSequence(int tierIndex)
    {
        yield return new WaitForSeconds(0.5f);

        if (maskLibrary.Count == 0)
        {
            Debug.LogError("GACHA: Mask Library is empty! Drag Archetypes into the list.");
            isSpinning = false;
            yield break;
        }

        // 1. Pick Archetype & Instantiate
        Mask archetype = maskLibrary[Random.Range(0, maskLibrary.Count)];
        Mask runtimeMask = Instantiate(archetype);

        // 2. Extract manual tier data
        Mask.RarityData selected = archetype.rarityTiers[tierIndex];
        
        runtimeMask.finalCuteness = selected.cutenessValue;
        runtimeMask.finalFear = selected.fearValue;
        runtimeMask.finalColor = selected.tierColor;
        runtimeMask.runtimeDisplayName = $"{selected.rarityLabel} {archetype.maskTypeName} Mask";

        // 3. Update UI
        string hexColor = ColorUtility.ToHtmlStringRGB(runtimeMask.finalColor);
        prizeText.text = $"<color=#{hexColor}>{runtimeMask.runtimeDisplayName}</color>\n" +
                         $"Cuteness: +{runtimeMask.finalCuteness} | Scardeyness: +{runtimeMask.finalFear}";
        
        prizeImage.sprite = runtimeMask.maskIcon;
        currentPendingMask = runtimeMask;
        // 4. Enable "Select Me" buttons on Team Slots

        foreach(var slot in TeamManager.Instance.slotUIs)
        {
            slot.SetButtonState(true); // Turn on the "Select Me" buttons!
        }
        prizePanel.SetActive(true);

        isSpinning = false; // Machine is ready for next spin
        Debug.Log($"GACHA: Prize Displayed - {runtimeMask.runtimeDisplayName}");
    }

    public void TrashCurrentPrize()
    {
        Debug.Log("<color=red>GACHA:</color> Prize trashed by player.");
        
        // 1. Hide the panel
        prizePanel.SetActive(false);
        
        // 2. Clear the UI fields (Good practice so old prizes don't "ghost" next time)
        prizeText.text = "";
        prizeImage.sprite = null;

        // 3. Ensure the machine is ready for a new spin if it wasn't already
        isSpinning = false;
        canSpin = true;
    }

    public void SelectCatForPrize(int index)
    {
        Debug.Log($"GACHA: Player selected Cat index {index} for prize.");
        // 1. Don't do anything if there's no prize waiting
        if (currentPendingMask == null) return;

        // 2. Tell the TeamManager to give the mask to the right cat
        TeamManager.Instance.EquipMaskToCat(index, currentPendingMask);

        // 3. Cleanup
        prizePanel.SetActive(false);
        currentPendingMask = null; // Clear it so it can't be added twice
        isSpinning = false;

        foreach(var slot in TeamManager.Instance.slotUIs)
        {
            slot.SetButtonState(false); // Hide buttons again
        }
        
        Debug.Log($"Gacha: Mask assigned to Cat {index}!");
    }
}