using UnityEngine;

public class GachaBehaviour : MonoBehaviour
{
    private bool canSpin = false;

    // This runs the moment the object is turned on/spawned
    void OnEnable()
    {
        Debug.Log("Gacha Ready! Press Enter to Spin.");
        canSpin = true; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpin && Input.GetKeyDown(KeyCode.Return))
        {
            SpinSlotMachine();
        }
    }

    public void SpinSlotMachine()
    {
        // Implementation for spinning the slot machine
        float spinResult = Random.Range(0f, 1f);
        Debug.Log("Slot Machine Spin Result: " + spinResult);

        // Animation and Sounds
        PickPrizePool(spinResult);
    }

    void PickPrizePool(float spinResult)
    {
        // Implementation for picking a prize based on spin result
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
        

        // Mask type selection
        string[] maskTypes = { "Ballroom", "Monocle", "Sunglasses" };

        int maskTypeIndex = Random.Range(0, maskTypes.Length);
        string selectedMaskType = maskTypes[maskTypeIndex];

        Debug.Log("Generating " + selectedMaskType + " mask with commonality: " + commonality);
    }
}
