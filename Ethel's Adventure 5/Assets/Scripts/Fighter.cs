using UnityEngine;
using UnityEngine.UI; // Required for Image
using TMPro;

public class Fighter : MonoBehaviour
{
    [Header("UI References")]
    public Image catImage;
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
        
        // Set Visuals
        catImage.sprite = data.catSprite;
        nameTag.text = data.catName;
    

        // Pull Stats from the persistent Cat instance
        currentCuteness = data.TotalCuteness;
        currentFear = data.TotalFear;

        UpdateUI();
    }

    public void UpdateUI()
    {
        cutenessTag.text = currentCuteness.ToString();
        fearTag.text = currentFear.ToString();
    }
}