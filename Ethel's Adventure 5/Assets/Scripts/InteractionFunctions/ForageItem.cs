using UnityEngine;
using System.Collections;

public class ForageItem : MonoBehaviour
{
   [Header("Wiggle Settings")]
    public float wiggleAmount = 0.1f;
    public float wiggleSpeed = 20f;
    public float duration = 0.5f;

    private bool isShaking = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void InteractionTrigger()
    {
        if (!isShaking)
        {
            StartCoroutine(WiggleAndGiveItem());
        }
    }

    private IEnumerator WiggleAndGiveItem()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Create a side-to-side shake effect using Sine
            float x = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
            transform.localPosition = originalPosition + new Vector3(x, 0, 0);
            yield return null;
        }

        // Reset to exact original position so the tree doesn't "drift"
        transform.localPosition = originalPosition;
        
        
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.AddFish(1);
            Debug.Log("The tree dropped a fish! Player now has: " + PlayerController.Instance.fishCount);
        }
        else
        {
            Debug.LogWarning("No PlayerController found in the scene to give fish to!");
        }
        
        isShaking = false;
    }
}
   