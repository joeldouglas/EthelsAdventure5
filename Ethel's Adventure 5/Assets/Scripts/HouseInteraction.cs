using UnityEngine;
using UnityEngine.InputSystem; // Add this line!

public class HouseInteractionSimple : MonoBehaviour
{
    [Header("Setup")]
    public GameObject questBubble; 
    [TextArea(3, 10)]
    public string houseDialog = "Hello! This is my house.";

    [Header("Settings")]
    public float proximity = 3f;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        if (questBubble != null) questBubble.SetActive(false);
    }

    void Update()
    {
        if (player == null || questBubble == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool isCloseEnough = dist < proximity;

        questBubble.SetActive(isCloseEnough);

        // NEW INPUT SYSTEM CHECK
        // Keyboard.current.spaceKey.wasPressedThisFrame is the new way to say GetKeyDown
        if (isCloseEnough && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ShowDialog();
        }
    }

    void ShowDialog()
    {
        Debug.Log("Triggering Dialog: " + houseDialog);
        // Your dialog logic goes here
    }
}