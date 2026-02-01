using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine.UI;

public class GachaBehaviour : MonoBehaviour
{
    [Header("Gacha Library")]
    [SerializeField] private List<Mask> maskLibrary; 

    [Header("UI References")]
    [SerializeField] private GameObject prizePanel;
    [SerializeField] private TextMeshProUGUI prizeText;
    [SerializeField] private Image prizeImage;
    [SerializeField] private GameObject SlotMachineUI; 

    [Header("Machine References")]
    [SerializeField] private GameObject handleObject;
    [SerializeField] private SpriteRenderer[] wheelSprites;
    
    // --- NEW MECHANICAL REFERENCES ---
    [Header("Mechanical Animation")]
    [SerializeField] private GameObject wheelBackground; // The object that will physically rotate
    [SerializeField] private float bgRotationMultiplier = 500f; // Tune this to make it spin faster/slower

    [Header("Settings")]
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private float spinDuration = 5f;

    private Mask currentPendingMask; 
    private bool isSpinning = false;
    private bool canSpin = true;
    private FightManager fightManager; 

    void Start()
    {
        fightManager = Object.FindAnyObjectByType<FightManager>();
        if(SlotMachineUI != null) SlotMachineUI.SetActive(false);
    }

    public void InitializeGacha()
    {
        if(SlotMachineUI != null) SlotMachineUI.SetActive(true);
        isEnabled = true;
        canSpin = true;
    }

    public void SpinSlotMachine()
    {
        if (isSpinning || !isEnabled || !canSpin) return;

        isSpinning = true;
        canSpin = false;
        if(prizePanel != null) prizePanel.SetActive(false); 

        StartCoroutine(SquashHandleEffect());
        StartCoroutine(WheelSpinEffect());
    }

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

    private IEnumerator WheelSpinEffect()
    {
        float timeTracker = 0f;
        float colorTicket = 0f;

        while (timeTracker < spinDuration)
        {
            timeTracker += Time.deltaTime;
            float progress = timeTracker / spinDuration;
            
            // 1. Calculate Flash/Spin Speed (Slows down at the end)
            float speedFactor = 1.0f;
            if (progress > 0.8f) 
                speedFactor = Mathf.Lerp(1.0f, 0.1f, (progress - 0.8f) / 0.2f);

            // 2. ROTATE BACKGROUND (NEW)
            if (wheelBackground != null)
            {
                // Rotates based on time, the multiplier, and slows down with speedFactor
                float rotationAmount = bgRotationMultiplier * speedFactor * Time.deltaTime;
                wheelBackground.transform.Rotate(0, 0, rotationAmount);
            }

            // 3. COLOR FLASHING LOGIC
            float currentFlashDelay = 0.05f / speedFactor;
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

        // Reset wheels to white
        foreach (SpriteRenderer wheel in wheelSprites)
        {
            if (wheel != null) wheel.color = Color.white;
        }

        // Rarity roll logic...
        float spinResult = Random.Range(0f, 1f);
        int tierIndex = 0;
        if (spinResult > 0.95f) tierIndex = 4;      
        else if (spinResult > 0.8f) tierIndex = 3;  
        else if (spinResult > 0.6f) tierIndex = 2;  
        else if (spinResult > 0.3f) tierIndex = 1;  
        else tierIndex = 0;                         

        StartCoroutine(ShowPrizeSequence(tierIndex));
    }

    // --- REST OF THE GACHA METHODS (ShowPrizeSequence, Cleanup, etc.) ---
    // (Keep the existing SelectCatForPrize and TrashCurrentPrize logic from your previous version)

    private IEnumerator ShowPrizeSequence(int tierIndex)
    {
        yield return new WaitForSeconds(0.5f);
        if (maskLibrary.Count == 0) { isSpinning = false; yield break; }

        Mask archetype = maskLibrary[Random.Range(0, maskLibrary.Count)];
        Mask runtimeMask = Instantiate(archetype);
        Mask.RarityData selected = archetype.rarityTiers[tierIndex];
        
        runtimeMask.finalCuteness = selected.cutenessValue;
        runtimeMask.finalFear = selected.fearValue;
        runtimeMask.finalColor = selected.tierColor;
        runtimeMask.runtimeDisplayName = $"{selected.rarityLabel} {archetype.maskTypeName} Mask";

        string hexColor = ColorUtility.ToHtmlStringRGB(runtimeMask.finalColor);
        if(prizeText != null)
            prizeText.text = $"<color=#{hexColor}>{runtimeMask.runtimeDisplayName}</color>\n" +
                             $"Cuteness: +{runtimeMask.finalCuteness} | Fear: +{runtimeMask.finalFear}";
        
        if(prizeImage != null) prizeImage.sprite = runtimeMask.maskIcon;
        currentPendingMask = runtimeMask;
        if(prizePanel != null) prizePanel.SetActive(true);

        TeamManager.Instance.SetTrayVisibility(true);
        foreach(var slot in TeamManager.Instance.slotUIs)
        {
            if(slot != null) slot.SetButtonState(true); 
        }
        isSpinning = false; 
    }

    public void TrashCurrentPrize()
    {
        CleanupGacha();
        if (fightManager != null) fightManager.OnGachaCompleted();
    }

    public void SelectCatForPrize(int index)
    {
        if (currentPendingMask == null) return;
        TeamManager.Instance.EquipMaskToCat(index, currentPendingMask);
        CleanupGacha();
        if (fightManager != null) fightManager.OnGachaCompleted();
    }

    private void CleanupGacha()
    {
        if(prizePanel != null) prizePanel.SetActive(false);
        currentPendingMask = null;
        isSpinning = false;
        canSpin = false;
        foreach(var slot in TeamManager.Instance.slotUIs)
        {
            if(slot != null) slot.SetButtonState(false); 
        }
        if(SlotMachineUI != null) SlotMachineUI.SetActive(false);
    }
}