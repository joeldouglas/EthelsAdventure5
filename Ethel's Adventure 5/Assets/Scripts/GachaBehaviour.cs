using UnityEngine;
using System.Collections;

public class GachaBehaviour : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private float spinDuration = 5f;

    [Header("References")]
    [SerializeField] private GameObject handleObject;
    [SerializeField] private SpriteRenderer[] wheelSprites; // Drag your 3 wheels here!

    private bool isSpinning = false;

    public void SpinSlotMachine()
    {
        if (isSpinning || !isEnabled)
        {
            Debug.Log("<color=yellow>GACHA:</color> Blocked! Already spinning or disabled.");
            return;
        }

        Debug.Log("<color=green>GACHA:</color> Spin Started!");
        isSpinning = true;

        // 1. Squash the handle (Short animation)
        StartCoroutine(SquashHandleEffect());

        // 2. Spin the wheels (Long animation)
        StartCoroutine(WheelSpinEffect());
    }

    private IEnumerator SquashHandleEffect()
    {
        if (handleObject == null)
        {
            Debug.LogError("GACHA: No Handle Object assigned!");
            yield break;
        }

        Debug.Log("GACHA: Squash Handle Animation Start.");
        Vector3 fullScale = Vector3.one;
        Vector3 squashedScale = new Vector3(1, 0.5f, 1);
        float halfTime = 0.25f;
        float elapsed = 0f;

        // Down
        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            handleObject.transform.localScale = Vector3.Lerp(fullScale, squashedScale, elapsed / halfTime);
            yield return null;
        }

        // Up
        elapsed = 0f;
        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            handleObject.transform.localScale = Vector3.Lerp(squashedScale, fullScale, elapsed / halfTime);
            yield return null;
        }

        handleObject.transform.localScale = fullScale;
        Debug.Log("GACHA: Squash Handle Animation Done.");
    }

   private IEnumerator WheelSpinEffect()
{
    Debug.Log("<color=cyan>GACHA:</color> Spin started. Timer is now frame-perfect.");
    
    float timeTracker = 0f;
    float colorTicket = 0f; // This tells us when it's time to change colors

    while (timeTracker < spinDuration)
    {
        timeTracker += Time.deltaTime; // The "Master Clock"
        Debug.Log("Time elapsed: " + timeTracker);
        
        // 1. CALCULATE DELAY (How long between flashes)
        float progress = timeTracker / spinDuration;
        float currentFlashDelay = 0.05f; // Default fast

        if (progress > 0.8f) 
        {
            // Smoothly slow down from 0.05s to 0.6s delay
            currentFlashDelay = Mathf.Lerp(0.05f, 0.6f, (progress - 0.8f) / 0.2f);
        }

        // 2. THE FLASH TIMER
        // We only change colors if enough time has passed since the last change
        colorTicket += Time.deltaTime;
        if (colorTicket >= currentFlashDelay)
        {
            foreach (SpriteRenderer wheel in wheelSprites)
            {
                if (wheel != null) 
                    wheel.color = new Color(Random.value, Random.value, Random.value);
            }
            colorTicket = 0f; // Reset the ticket for the next flash
        }

        yield return null; // Wait exactly ONE frame and check the loop again
    }

    // FINAL RESET
    foreach (SpriteRenderer wheel in wheelSprites)
    {
        if (wheel != null) wheel.color = Color.white;
    }

    Debug.Log("<color=cyan>GACHA:</color> Done! Real time elapsed: " + timeTracker);
    
    isSpinning = false;
    float spinResult = Random.Range(0f, 1f);
    PickPrizePool(spinResult);
}

    void PickPrizePool(float spinResult)
    {
        if (spinResult < 0.5f)
        {
            Debug.Log("You won a common prize!");
            GenerateMask(1);
        }
        else if (spinResult < 0.8f)
        {
            Debug.Log("You won a rare prize!");
            GenerateMask(2);
        }
        else
        {
            Debug.Log("You won a legendary prize!");
            GenerateMask(3);
        }
    }

    void GenerateMask(int commonality)
    {
        string[] maskTypes = { "Ballroom", "Monocle", "Sunglasses" };
        string selectedMaskType = maskTypes[Random.Range(0, maskTypes.Length)];

        Debug.Log($"Generating {selectedMaskType} mask with commonality: {commonality}");
    }

  
}
