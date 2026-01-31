using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSlotUI : MonoBehaviour
{
    // --- ADD THIS LINE ---
    [Tooltip("Set this to 0, 1, or 2 in the Inspector to match the Team List")]
    public int slotIndex; 
    // ---------------------

    [Header("Visual Elements")]
    public Image catIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI cutenessText; 
    public TextMeshProUGUI fearText; 

    [Header("Controls")]
    public Button selectButton; 

    public void Refresh(Cat liveCat)
    {
        if (liveCat == null) return;
        
        catIcon.sprite = liveCat.catSprite;
        nameText.text = liveCat.catName;
        cutenessText.text = liveCat.TotalCuteness.ToString();
        fearText.text = liveCat.TotalFear.ToString();
    }

    public void SetButtonState(bool active)
    {
        if (selectButton != null) 
            selectButton.gameObject.SetActive(active);
    }
}