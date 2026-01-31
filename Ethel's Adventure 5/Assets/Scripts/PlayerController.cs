using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // --- SINGLETON REFERENCE ---
    public static PlayerController Instance { get; private set; }

    [Header("Settings")]
    public float speed = 8f;

    [Header("State")]
    public bool inFight = false;
    public bool canMove = true;

    [Header("References")]
    [SerializeField] private GachaBehaviour gacha;
    [SerializeField] private SpriteRenderer playerSprite;

    private Rigidbody2D rb;
    private Vector2 moveValue;

    private void Awake()
    {
        // --- DDOL SINGLETON LOGIC ---
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Ensure it's a root object for DDOL
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If a player already exists (from a previous scene), teleport the 
            // persistent one to this temporary one's position, then destroy this one.
            Instance.transform.position = this.transform.position;
            Instance.transform.rotation = this.transform.rotation;
            
            Destroy(gameObject);
            return;
        }

        // Initialize components for the very first instance
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Every time we change scenes, check if we should be visible/moving
        HandleSceneContext(scene.buildIndex);
        
        // Re-find the Gacha if we are in the Lobby scene
        if (scene.name == "CatsinoLobby")
        {
            gacha = Object.FindAnyObjectByType<GachaBehaviour>();
        }
    }

    void Start()
    {
        // Initial setup for the first time the game runs
        HandleSceneContext(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandleSceneContext(int sceneIndex)
    {
        // Build Index 4 = Prize/Gacha Scene
        if (sceneIndex == 4)
        {
            if (playerSprite != null) playerSprite.enabled = false;
            canMove = false;
        }
        else
        {
            if (playerSprite != null) playerSprite.enabled = true;
            canMove = true;
        }
    }

    void Update()
    {
        // 1. MOVEMENT LOGIC
        if (canMove)
        {
            float x = 0;
            float y = 0;

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

        if (rb != null)
        {
            rb.linearVelocity = moveValue * speed;
        }

        // 2. INTERACTION LOGIC
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            AttemptGachaSpin();
        }
    }

    void AttemptGachaSpin()
    {
        if (CanInteractWithGacha())
        {
            Debug.Log("Attempting to spin Gacha.");
            gacha.SpinSlotMachine();
        }
    }

    bool CanInteractWithGacha()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene != "CatsinoLobby") return false;
        if (inFight) return false;
        if (gacha == null) return false;
        
        return true;
    }
}