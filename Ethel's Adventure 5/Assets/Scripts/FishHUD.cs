using UnityEngine;
using TMPro;

public class FishHUD : MonoBehaviour
{
    private TextMeshProUGUI fishText;

    void Awake()
    {
        fishText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // The UI "observes" the PlayerController and mimics its value
        if (PlayerController.Instance != null && fishText != null)
        {
            fishText.text = PlayerController.Instance.fishCount.ToString();
        }
    }
}