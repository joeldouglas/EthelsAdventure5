using UnityEngine;
using UnityEngine.InputSystem; // Still needed for Keyboard detection
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 8f;

    [Header("State")]
    public bool inFight = false;
    public bool canMove = true;

    [Header("References")]
    [SerializeField] private GachaBehaviour gacha;

    private Rigidbody2D rb;
    private Vector2 moveValue;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        Debug.Log("Player Initialized. Movement set to 4-way.");
    }

    void Update()
    {
        // 1. MOVEMENT LOGIC
        if (canMove)
        {
            float x = 0;
            float y = 0;

            // Direct keyboard checks (No Input Action Asset needed for these!)
            if (Keyboard.current.wKey.isPressed) y = 1;
            if (Keyboard.current.sKey.isPressed) y = -1;
            if (Keyboard.current.aKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed) x = 1;

            moveValue = new Vector2(x, y).normalized;
        }
        else
        {
            moveValue = Vector2.zero;
        }

        rb.linearVelocity = moveValue * speed;

        // 2. INTERACTION LOGIC
        // We check for Spacebar or Enter directly
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log("Manual Input Check: Space/Enter Pressed!");
            AttemptGachaSpin();
        }
    }

    void AttemptGachaSpin()
    {
        if (CanInteractWithGacha())
        {
            Debug.Log("All checks passed! Attempting to spin Gacha.");
            gacha.SpinSlotMachine();
        }
    }

    bool CanInteractWithGacha()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        // DEBUG: This will tell us if the scene name is the problem
        if (currentScene != "CatsinoLobby") 
        {
            Debug.LogWarning("Interaction Failed: Scene name is " + currentScene + " but we need CatsinoLobby");
            return false;
        }

        if (inFight) return false;
        if (gacha == null) { Debug.LogError("Gacha reference is MISSING!"); return false; }
        
        return true;
    }
}