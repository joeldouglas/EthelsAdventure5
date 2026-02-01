using UnityEngine;

public class HouseInteraction : MonoBehaviour
{
     public string houseDialog = "Hello! This is my house.";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void ShowDialog()
    {
        Debug.Log("Triggering Dialog: " + houseDialog);
        // Your dialog logic goes here
    }
}
