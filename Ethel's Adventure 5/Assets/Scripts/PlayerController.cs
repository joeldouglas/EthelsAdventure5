using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // --- SINGLETON REFERENCE ---
    public static PlayerController Instance { get; private set; }

    [Header("Settings")]
    public float speed = 8f;
    public bool canMove = true;

    [Header("References")]
    [SerializeField] private SpriteRenderer playerSprite;
    
    // Internal Variables
    private Rigidbody2D rb;
    private Vector2 moveValue;
    
    // Reference cache for Scene 4
    private FightManager currentFightManager; 

    public int currentLevel = 1; // This is the level the player has reached

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance.transform.position = this.transform.position;
            Instance.transform.rotation = this.transform.rotation;
            Destroy(gameObject);
            return;
        }

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
        HandleSceneContext(scene.buildIndex);
    }

    void Start()
    {
        HandleSceneContext(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandleSceneContext(int sceneIndex)
    {
        // Build Index 4 = Catsino (Scene 4)
        if (sceneIndex == 4)
        {
            // Hide Player Visuals in Casino/Fight
            if (playerSprite != null) playerSprite.enabled = false;
            canMove = false;
            
            // Try to find the FightManager immediately
            currentFightManager = Object.FindAnyObjectByType<FightManager>();
        }
        else
        {
            // Show Player Visuals in Lobby/World
            if (playerSprite != null) playerSprite.enabled = true;
            canMove = true;
            currentFightManager = null;
        }
    }

    void Update()
    {
        if (currentLevel > 3)
        {
            EndGame();
        }
        // 1. MOVEMENT
        if (canMove)
        {
            float x = 0;
            float y = 0;

            if (Keyboard.current.wKey.isPressed) y = 1;
            if (Keyboard.current.sKey.isPressed) y = -1;
            if (Keyboard.current.aKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed) x = 1;

            moveValue = new Vector2(x, y).normalized;
            if (rb != null) rb.linearVelocity = moveValue * speed;
        }
        else
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // 2. INTERACTION
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (sceneIndex == 4) // CATSINO SCENE
            {
                // If we lost reference (recompile/reload), find it again
                if (currentFightManager == null) 
                    currentFightManager = Object.FindAnyObjectByType<FightManager>();
                
                if (currentFightManager != null)
                {
                    currentFightManager.OnSpacePressed();
                }
            }
            else // LOBBY OR OTHER SCENES
            {
                // Put standard Lobby interaction logic here if needed
                // e.g. opening doors, talking to NPCs
            }
        }
    }

    private void EndGame()
    {
        // Transition to End Game Scene (Build Index 5)
        SceneTransitions.Instance.TransitionTo(9);
    }
}