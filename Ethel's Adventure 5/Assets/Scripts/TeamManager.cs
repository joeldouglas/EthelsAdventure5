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

    private void Start()
    {
        HandleInitialVisibility();
    }

    // This runs every time a new scene is loaded to ensure the tray is right
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
        // Re-find the UI in the new scene since the old UI objects were destroyed
        FindUIInScene();
        HandleInitialVisibility();
    }

    private void HandleInitialVisibility()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        UpdateUI();

        // Build Index 4 = Prize/Gacha Scene
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
        // In a persistent system, we need to find the new UI tray and slots 
        // because the ones dragged into the inspector at Start are now gone.
        // Look for the tray panel tag or name
        if (trayPanel == null) trayPanel = GameObject.Find("CatTeamPanel");
        
        // Find all slots in the scene and sort them
// Change FindObjectSortMode.None to UnityEngine.FindObjectSortMode.None
        slotUIs = Object.FindObjectsOfType<TeamSlotUI>();

        // Sort slots by their internal slotIndex to make sure they match myTeam
        System.Array.Sort(slotUIs, (a, b) => a.slotIndex.CompareTo(b.slotIndex));
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
                myTeam.Add(Instantiate(startingCats[i]));
            }
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slotUIs == null || slotUIs.Length == 0) return;

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