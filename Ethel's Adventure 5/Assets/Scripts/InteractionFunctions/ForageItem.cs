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
            // Create a random offset for the "shake" effect
            float x = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
            transform.localPosition = originalPosition + new Vector3(x, 0, 0);
            yield return null;
        }

        transform.localPosition = originalPosition;
        
        // Give the item
        //InventoryManager.Instance.AddFish(1);
        
        isShaking = false;
        Debug.Log("The forageable dropped a fish!");
    }
}
   