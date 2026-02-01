// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.SceneManagement;

// public class PlayerController : MonoBehaviour
// {
//     // --- SINGLETON REFERENCE ---
//     public static PlayerController Instance { get; private set; }

//     [Header("Settings")]
//     public float speed = 8f;

//     [Header("State")]
//     public bool inFight = false;
//     public bool canMove = true;

//     [Header("References")]
//     [SerializeField] private GachaBehaviour gacha;
//     [SerializeField] private SpriteRenderer playerSprite;

//     private Rigidbody2D rb;
//     private Vector2 moveValue;
//     public bool canBattle = false;

//     private void Awake()
//     {
//         // --- DDOL SINGLETON LOGIC ---
//         if (Instance == null)
//         {
//             Instance = this;
//             transform.SetParent(null); // Ensure it's a root object for DDOL
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             // If a player already exists (from a previous scene), teleport the 
//             // persistent one to this temporary one's position, then destroy this one.
//             Instance.transform.position = this.transform.position;
//             Instance.transform.rotation = this.transform.rotation;
            
//             Destroy(gameObject);
//             return;
//         }

//         // Initialize components for the very first instance
//         rb = GetComponent<Rigidbody2D>();
//         if (rb != null) rb.gravityScale = 0;
//     }

//     private void OnEnable()
//     {
//         SceneManager.sceneLoaded += OnSceneLoaded;
//     }

//     private void OnDisable()
//     {
//         SceneManager.sceneLoaded -= OnSceneLoaded;
//     }

//     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         // Every time we change scenes, check if we should be visible/moving
//         HandleSceneContext(scene.buildIndex);
        
//         // Re-find the Gacha if we are in the Lobby scene
//         if (scene.name == "CatsinoLobby")
//         {
//             gacha = Object.FindAnyObjectByType<GachaBehaviour>();
//             canBattle = true;

//         }
//     }

//     void Start()
//     {
//         // Initial setup for the first time the game runs
//         HandleSceneContext(SceneManager.GetActiveScene().buildIndex);
//     }

//     private void HandleSceneContext(int sceneIndex)
//     {
//         // Build Index 4 = Prize/Gacha Scene
//         if (sceneIndex == 4)
//         {
//             if (playerSprite != null) playerSprite.enabled = false;
//             canMove = false;
//         }
//         else
//         {
//             if (playerSprite != null) playerSprite.enabled = true;
//             canMove = true;
//         }
//     }

//     void Update()
//     {
//         // 1. MOVEMENT LOGIC
//         if (canMove)
//         {
//             float x = 0;
//             float y = 0;

//             if (Keyboard.current.wKey.isPressed) y = 1;
//             if (Keyboard.current.sKey.isPressed) y = -1;
//             if (Keyboard.current.aKey.isPressed) x = -1;
//             if (Keyboard.current.dKey.isPressed) x = 1;

//             moveValue = new Vector2(x, y).normalized;
//         }
//         else
//         {
//             moveValue = Vector2.zero;
//         }

//         if (rb != null)
//         {
//             rb.linearVelocity = moveValue * speed;
//         }

//         // 2. INTERACTION LOGIC
//         if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
//         {
//             AttemptGachaSpin();
//         }
//     }

//     void AttemptGachaSpin()
//     {
//         if (CanInteractWithGacha())
//         {
//             Debug.Log("Attempting to spin Gacha.");
//             gacha.SpinSlotMachine();
//         }
//     }

//     bool CanInteractWithGacha()
//     {
//         string currentScene = SceneManager.GetActiveScene().name;
        
//         if (currentScene != "CatsinoLobby") return false;
//         if (inFight) return false;
//         if (gacha == null) return false;
        
//         return true;
//     }
// }

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
}