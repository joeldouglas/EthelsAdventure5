using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour
{
    // --- SINGLETON REFERENCE ---
    public static TeamManager Instance;

    [Header("Team Setup")]
    [SerializeField] private List<Cat> startingCats; 
    
    // Using NonSerialized so Unity doesn't try to draw the live list in the Inspector
    [System.NonSerialized] public List<Cat> myTeam = new List<Cat>(); 

    [Header("UI References")]
    public TeamSlotUI[] slotUIs; 

    [Header("Visibility")]
    [SerializeField] private GameObject trayPanel; 

    private void Awake()
    {
        // --- DDOL SINGLETON LOGIC ---
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Ensure it's a root object for DDOL
            DontDestroyOnLoad(gameObject);
            InitializeTeam();
        }
        else
        {
            // If a manager already exists, destroy this one
            Destroy(gameObject);
            return;
        }
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
        // Every time we enter a new scene, re-link the UI
        FindUIInScene();
        HandleInitialVisibility();
    }

    private void Start()
    {
        // Initial setup for the first time the game runs
        FindUIInScene();
        HandleInitialVisibility();
    }

    private void HandleInitialVisibility()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        UpdateUI();

        // Build Index 4 = Prize/Gacha Scene (Hide tray initially)
        if (sceneIndex == 4)
        {
            SetTrayVisibility(false);
        }
        else
        {
            SetTrayVisibility(true);
        }
    }

   private void FindUIInScene()
{
    // 1. Find the Canvas (which must be ACTIVE)
    Canvas mainCanvas = Object.FindAnyObjectByType<Canvas>();
    if (mainCanvas == null) return;

    // 2. Search all children of the Canvas, INCLUDING inactive ones
    TeamSlotUI[] allSlots = mainCanvas.GetComponentsInChildren<TeamSlotUI>(true);

    // 3. Initialize our array
    slotUIs = new TeamSlotUI[3];

    // 4. Map them by their name or SlotIndex
    foreach (TeamSlotUI slot in allSlots)
    {
        if (slot.gameObject.name == "LeftCat") slotUIs[0] = slot;
        else if (slot.gameObject.name == "MiddleCat") slotUIs[1] = slot;
        else if (slot.gameObject.name == "RightCat") slotUIs[2] = slot;
    }
    
    // 5. Find the Tray Panel (also including inactive)
    if (trayPanel == null)
    {
        Transform trayTransform = mainCanvas.transform.Find("CatTeamPanel");
        if (trayTransform != null) trayPanel = trayTransform.gameObject;
    }
}

    public void SetTrayVisibility(bool isVisible)
    {
        if (slotUIs != null)
        {
            foreach (var slot in slotUIs)
            {
                if (slot != null) slot.SetButtonState(isVisible);
            }
        }
        
        if (trayPanel != null)
        {
            trayPanel.SetActive(isVisible);
        }
    }

    private void InitializeTeam()
    {
        myTeam.Clear();
        for (int i = 0; i < startingCats.Count; i++)
        {
            if (startingCats[i] != null)
            {
                // Create the live instances of your cats
                myTeam.Add(Instantiate(startingCats[i]));
            }
        }
    }

    public void UpdateUI()
    {
        // If slots are missing (scene change), try to find them
        if (slotUIs == null || slotUIs.Length == 0 || slotUIs[0] == null) 
            FindUIInScene();

        for (int i = 0; i < myTeam.Count; i++)
        {
            if (i < slotUIs.Length && slotUIs[i] != null)
            {
                slotUIs[i].Refresh(myTeam[i]);
            }
        }
    }

    public void EquipMaskToCat(int index, Mask mask)
    {
        if (index >= 0 && index < myTeam.Count)
        {
            myTeam[index].equippedMask = mask;
            UpdateUI();
        }
    }
}