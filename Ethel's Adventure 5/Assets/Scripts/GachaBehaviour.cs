using UnityEngine;

public class GachaBehaviour : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;

    public bool IsEnabled => isEnabled;

    void Awake()
{
    // This is your "Hello" message. If you don't see this, 
    // the script isn't on any object in the scene!
    Debug.Log("GACHA SYSTEM: Awake and initialized.");
}


    public void SpinSlotMachine()
    {
        float spinResult = Random.Range(0f, 1f);
        Debug.Log("Slot Machine Spin Result: " + spinResult);

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
