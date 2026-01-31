using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSlotUI : MonoBehaviour
{
    [Header("Visual Elements")]
    public Image catIcon;      // Drag the "Cat Sprite" UI Image here
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI cutenessText; 
    public TextMeshProUGUI fearText; 
    
    [Header("Controls")]
    public GameObject selectButton; // Drag the Button's GameObject here

    public void Refresh(Cat liveCat)
    {
        if (liveCat == null) return;
        
        // Push the data to the fields
        catIcon.sprite = liveCat.catSprite;
        nameText.text = liveCat.catName;
        cutenessText.text = liveCat.TotalCuteness.ToString();
        fearText.text = liveCat.TotalFear.ToString();

        // Ensure the sprite is visible even if button is off
        catIcon.enabled = true; 
    }

    // Call this from Gacha to show/hide the buttons during prize selection
    public void SetButtonState(bool active)
    {
        if(selectButton != null) selectButton.GetComponent<UnityEngine.UI.Button>().interactable = active;

    }
}