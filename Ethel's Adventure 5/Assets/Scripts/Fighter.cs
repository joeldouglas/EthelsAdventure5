using UnityEngine;
using UnityEngine.UI; // Required for Image
using TMPro;

public class Fighter : MonoBehaviour
{
    [Header("UI References")]
    public Image catImage;
    public Image maskOverlay; // <--- Drag your Mask Image here in Inspector
    
    public TextMeshProUGUI nameTag;
    public TextMeshProUGUI cutenessTag;
    public TextMeshProUGUI fearTag;

    [Header("Live Stats")]
    public int currentCuteness;
    public int currentFear;
    public bool isPlayer;

    public void Initialize(Cat data, bool _isPlayer)
    {
        isPlayer = _isPlayer;
        
        // 1. Set Cat Visuals
        if (catImage != null)
        {
            catImage.sprite = data.catSprite;
        }

        if (nameTag != null) nameTag.text = data.catName;
    
        // 2. Set Mask Visuals
        if (maskOverlay != null)
        {
            if (data.equippedMask != null)
            {
                // We have a mask! Turn it on and set visuals
                maskOverlay.sprite = data.equippedMask.maskIcon;
                maskOverlay.color = data.equippedMask.finalColor;
                maskOverlay.enabled = true;
            }
            else
            {
                // No mask? Hide the overlay
                maskOverlay.enabled = false;
            }
        }

        // 3. Pull Stats 
        // Note: data.TotalCuteness now automatically includes mask stats from Cat.cs
        currentCuteness = data.TotalCuteness;
        currentFear = data.TotalFear;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (cutenessTag != null) cutenessTag.text = currentCuteness.ToString();
        if (fearTag != null) fearTag.text = currentFear.ToString();
    }
}