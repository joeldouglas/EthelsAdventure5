using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSlotUI : MonoBehaviour
{
    [Header("Visuals")]
    public Image catImage;       
    public Image maskOverlay;    // Ensure this is linked!

    [Header("Text")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI cutenessText; // New separate field
    public TextMeshProUGUI fearText;     // New separate field

    [Header("Interaction")]
    public Button selectButton; 

    public void Refresh(Cat catData)
    {
        // 1. Update Visuals
        if (catData != null)
        {
            // Cat
            if (catImage != null)
            {
                catImage.sprite = catData.catSprite;
                catImage.enabled = true;
            }

            // Mask
            if (maskOverlay != null)
            {
                if (catData.equippedMask != null)
                {
                    maskOverlay.sprite = catData.equippedMask.maskIcon;
                    maskOverlay.enabled = true;
                    maskOverlay.color = catData.equippedMask.finalColor; 
                }
                else
                {
                    maskOverlay.enabled = false;
                }
            }

            // Name
            if (nameText != null) nameText.text = catData.catName;
            
            // Stats (Separated)
            if (cutenessText != null) cutenessText.text = catData.TotalCuteness.ToString();
            if (fearText != null) fearText.text = catData.TotalFear.ToString();
        }
        else
        {
            // Handle Empty Slot
            if (catImage != null) catImage.enabled = false;
            if (maskOverlay != null) maskOverlay.enabled = false;
            if (nameText != null) nameText.text = "Empty";
            
            if (cutenessText != null) cutenessText.text = "";
            if (fearText != null) fearText.text = "";
        }

        // 2. Reset Button (Always start hidden until Gacha asks for it)
        if (selectButton != null)
        {
            selectButton.interactable = false;
            selectButton.gameObject.SetActive(false); 
        }
    }

    // Called by GachaBehaviour
    public void SetButtonState(bool isActive)
    {
        if (selectButton != null)
        {
            selectButton.gameObject.SetActive(isActive);
            selectButton.interactable = isActive;
        }
    }
}