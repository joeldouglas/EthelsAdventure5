using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.Events;

public class UniversalInteraction : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject interactableUI; 
    public float proximity = 3f;

    [Header("The Interaction")]
    public UnityEvent onInteract; 

    private Transform player;

    void Start()
    {
        //Finds player location
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        
        //Sets UI to off.
        if (interactableUI != null) interactableUI.SetActive(false);
    }

    void Update()
    {
        if (player == null || interactableUI == null) return;

        //Finds proximity and sees if player is close enough
        float dist = Vector2.Distance(transform.position, player.position);
        bool isCloseEnough = dist < proximity;

        interactableUI.SetActive(isCloseEnough);

        // Check for Spacebar press
        if (isCloseEnough && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (PlayerController.Instance.canInteract)            
                onInteract.Invoke(); 
        }
    }
}