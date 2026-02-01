using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    [Header("Team Setup")]
    [SerializeField] private List<Cat> startingCats; 
    
    // Live List of Cats
    public List<Cat> myTeam = new List<Cat>(); 

    [Header("UI References")]
    public TeamSlotUI[] slotUIs = new TeamSlotUI[3]; 
    public GameObject trayPanel; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitializeTeam();
        }
        else
        {
            Destroy(gameObject);
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
        // 1. Clear old broken references immediately
        slotUIs = new TeamSlotUI[3]; 
        trayPanel = null;

        // 2. Find the new ones
        FindUIInScene();
        HandleInitialVisibility();
    }

    private void Start()
    {
        FindUIInScene();
        HandleInitialVisibility();
    }

    private void InitializeTeam()
    {
        myTeam.Clear();
        for (int i = 0; i < startingCats.Count; i++)
        {
            if (startingCats[i] != null) myTeam.Add(Instantiate(startingCats[i]));
        }
    }

    private void HandleInitialVisibility()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Scene 4 = Battle/Catsino (Hide tray by default)
        if (sceneIndex == 4) 
        {
            SetTrayVisibility(false);
        }
        else 
        {
            SetTrayVisibility(true);
            UpdateUI();
        }
    }

    // --- THE FIX: ROBUST FINDING LOGIC ---
    public void FindUIInScene()
    {
        // 1. Find the Main Canvas (Active or Inactive)
        // We look for *any* TeamSlotUI component in the scene to find the right container
        // This is safer than looking for "Canvas" if you have multiple canvases
        TeamSlotUI foundSlot = Object.FindFirstObjectByType<TeamSlotUI>();
        
        if (foundSlot != null)
        {
            // If we found one slot, we can find the others in the same parent/canvas
            Canvas rootCanvas = foundSlot.GetComponentInParent<Canvas>();
            if (rootCanvas == null) return;

            // Search ALL children, including inactive ones (True)
            TeamSlotUI[] allSlots = rootCanvas.GetComponentsInChildren<TeamSlotUI>(true);
            
            // Map them by name
            slotUIs = new TeamSlotUI[3];
            foreach (var slot in allSlots)
            {
                if (slot.name == "LeftCat") slotUIs[0] = slot;
                else if (slot.name == "MiddleCat") slotUIs[1] = slot;
                else if (slot.name == "RightCat") slotUIs[2] = slot;
            }

            // Find the Tray Panel
            Transform[] allTransforms = rootCanvas.GetComponentsInChildren<Transform>(true);
            foreach(Transform t in allTransforms)
            {
                if (t.name == "CatTeamPanel")
                {
                    trayPanel = t.gameObject;
                    break;
                }
            }
        }
    }

    // --- THE FIX: SELF-HEALING UPDATE ---
    public void UpdateUI()
    {
        // 1. CHECK FOR BROKEN REFERENCES
        // If the first slot is null, or the reference acts like null (Unity Object check), re-find them.
        if (slotUIs == null || slotUIs.Length < 3 || slotUIs[0] == null) 
        {
            Debug.Log("TeamManager: Lost UI references. Finding them now...");
            FindUIInScene();
        }

        // 2. Push Data
        if (slotUIs != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (slotUIs[i] != null)
                {
                    if (i < myTeam.Count && myTeam[i] != null)
                    {
                        slotUIs[i].Refresh(myTeam[i]);
                    }
                    else
                    {
                        slotUIs[i].Refresh(null); 
                    }
                }
            }
        }
    }

    public void SetTrayVisibility(bool isVisible)
    {
        if (trayPanel == null) FindUIInScene();

        if (trayPanel != null)
        {
            trayPanel.SetActive(isVisible);
        }
    }

    public void EquipMaskToCat(int index, Mask mask)
    {
        if (index >= 0 && index < myTeam.Count && myTeam[index] != null)
        {
            myTeam[index].EquipMask(mask);
            UpdateUI();
        }
    }
}