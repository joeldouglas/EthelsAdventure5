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
        // Changes display to current fish count.
        if (PlayerController.Instance != null && fishText != null)
        {
            fishText.text = PlayerController.Instance.fishCount.ToString();
        }
    }
}