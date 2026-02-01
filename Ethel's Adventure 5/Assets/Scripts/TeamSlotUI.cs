using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSlotUI : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Set this in Inspector! 0=Left, 1=Middle, 2=Right")]
    public int slotIndex; 

    [Header("Visuals")]
    public Image catImage;       
    public Image maskOverlay;    

    [Header("Text")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI cutenessText; 
    public TextMeshProUGUI fearText;     

    [Header("Interaction")]
    public Button selectButton; 

    private void Start()
    {
        // 1. SELF-REGISTRATION
        if (TeamManager.Instance != null)
        {
            TeamManager.Instance.RegisterSlot(this, slotIndex);
        }

        // 2. DYNAMIC BUTTON WIRING (The Fix)
        // We set up the click listener via code so it doesn't break across scenes.
        if (selectButton != null)
        {
            // Remove old listeners to prevent double-clicks
            selectButton.onClick.RemoveAllListeners(); 
            selectButton.onClick.AddListener(OnSlotClicked);
        }
    }

    // This runs when the player clicks the button
    private void OnSlotClicked()
    {
        // Find the Gacha script in the current scene
        GachaBehaviour gacha = Object.FindAnyObjectByType<GachaBehaviour>();
        
        if (gacha != null)
        {
            // Tell the Gacha "I was chosen!"
            gacha.SelectCatForPrize(slotIndex);
        }
        else
        {
            Debug.LogWarning("TeamSlotUI: Clicked, but no GachaBehaviour found in scene!");
        }
    }

    public void Refresh(Cat catData)
    {
        // Debugging check
        if (maskOverlay == null) Debug.LogError($"{gameObject.name}: MaskOverlay Image is MISSING in Inspector!");
        
        if (catData != null)
        {
            // Update Cat Image
            if (catImage != null)
            {
                catImage.sprite = catData.catSprite;
                catImage.enabled = true;
            }

            // Update Mask
            if (maskOverlay != null)
            {
                if (catData.equippedMask != null)
                {
                    maskOverlay.sprite = catData.equippedMask.maskIcon;
                    maskOverlay.color = catData.equippedMask.finalColor;
                    
                    // Force Alpha to 1
                    Color c = maskOverlay.color;
                    c.a = 1f; 
                    maskOverlay.color = c;
                    
                    maskOverlay.enabled = true; 
                }
                else
                {
                    maskOverlay.enabled = false;
                }
            }

            // Update Text
            if (nameText != null) nameText.text = catData.catName;
            if (cutenessText != null) cutenessText.text = catData.TotalCuteness.ToString();
            if (fearText != null) fearText.text = catData.TotalFear.ToString();
        }
        else
        {
            // Empty Slot
            if (catImage != null) catImage.enabled = false;
            if (maskOverlay != null) maskOverlay.enabled = false;
            if (nameText != null) nameText.text = "Empty";
            if (cutenessText != null) cutenessText.text = "";
            if (fearText != null) fearText.text = "";
        }

        // Reset Button State (Default to off until Gacha turns it on)
        if (selectButton != null) selectButton.interactable = false; 
    }

    public void SetButtonState(bool isActive)
    {
        if (selectButton != null) selectButton.interactable = isActive;
    }
}