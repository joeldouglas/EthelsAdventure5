using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // --- SINGLETON REFERENCE ---
    public static PlayerController Instance { get; private set; }

    [Header("Settings")]
    public float speed = 8f;
    public bool canMove = true;
    public bool canInteract = true;

    [Header("Inventory")]
    public int fishCount = 0; // The fish tally

    [Header("References")]
    [SerializeField] private SpriteRenderer playerSprite;
    
    // Internal Variables
    private Rigidbody2D rb;
    private Vector2 moveValue;
    
    // Reference cache for Scene 4
    private FightManager currentFightManager; 

    public bool start = true;
    public bool instructions = false;


    public int currentLevel = 1; // This is the level the player has reached

    [Header("Sprites")]
    public SpriteRenderer renderer;
    public Sprite currentSprite;
    public Sprite spriteLeft;
    public Sprite spriteRight;


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


        // randomly set start sprite
        currentSprite = Random.Range(0,2) == 0 ? spriteLeft : spriteRight;
        renderer.sprite = currentSprite;


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
        if(sceneIndex == 0 && start)
        {
            canMove = false;
            GameObject.Find("Start").SetActive(true);
            GameObject.Find("Instructions").SetActive(true);
        }
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
            if (Keyboard.current.aKey.isPressed)
            {
                x = -1;

                // swap sprite to left
                if (currentSprite != spriteLeft)
                {
                    currentSprite = spriteLeft;
                    renderer.sprite = currentSprite;
                }
            }
            if (Keyboard.current.dKey.isPressed)
            {
                x = 1;

                // swap sprite to right
                if (currentSprite != spriteRight)
                {
                    currentSprite = spriteRight;
                    renderer.sprite = currentSprite;
                }

            }

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

            if(sceneIndex == 0)
            {
                if (start)
                {
                    instructions = true;
                    start = false;
                    GameObject.Find("Start").SetActive(false);
                }
                else if (instructions)
                {
                    instructions = false;
                    canMove = true;
                    GameObject.Find("Instructions").SetActive(false);
                }
            }

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
    // --- FISH METHODS ---

    public void AddFish(int amount)
    {
        fishCount += amount;
        Debug.Log("Fish collected! Total: " + fishCount);
    }


    private void EndGame()
    {
        // Transition to End Game Scene (Build Index 5)
        SceneTransitions.Instance.TransitionTo(9);
    }
}