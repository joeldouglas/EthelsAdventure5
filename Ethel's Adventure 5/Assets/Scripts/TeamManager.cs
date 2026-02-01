using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    [Header("Team Setup")]
    [SerializeField] private List<Cat> startingCats; 
    public List<Cat> myTeam = new List<Cat>(); 

    [Header("UI References (Auto-Linked via Registration)")]
    public TeamSlotUI[] slotUIs = new TeamSlotUI[3]; 
    public GameObject trayPanel; 

    [Header("Current Team Status (Read-Only)")]
    public List<string> teamStatusView; 

    [Header("Debug Controls")]
    public Mask debugMaskToEquip;
    [Range(0, 2)] public int debugCatIndex = 0;
    public DebugRarity debugRarity = DebugRarity.Common;
    public enum DebugRarity { Common = 0, Uncommon = 1, Rare = 2, VeryRare = 3, Legendary = 4 }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitializeTeam();
        }
        else Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear references on load
        slotUIs = new TeamSlotUI[3];
        trayPanel = null;
        
        FindTrayPanel(); // Still need to find the panel container
        HandleInitialVisibility();
        UpdateDebugView();
    }

    private void Start()
    {
        FindTrayPanel();
        HandleInitialVisibility();
    }

    // --- INITIALIZATION (Includes Starting Masks) ---
    private void InitializeTeam()
    {
        myTeam.Clear();
        for (int i = 0; i < startingCats.Count; i++)
        {
            if (startingCats[i] != null) 
            {
                // 1. Create a Runtime Copy of the Cat
                Cat newCat = Instantiate(startingCats[i]);

                // 2. Handle Starting Masks
                // If the ScriptableObject had a mask assigned in the inspector...
                if (newCat.equippedMask != null)
                {
                    // Create a runtime copy of that mask so we don't edit the asset
                    Mask newMask = Instantiate(newCat.equippedMask);

                    // Initialize the mask with default stats (Tier 0 / Common)
                    // This ensures the numbers aren't 0
                    if (newMask.rarityTiers != null && newMask.rarityTiers.Length > 0)
                    {
                        Mask.RarityData data = newMask.rarityTiers[0]; // Default to Common
                        newMask.finalCuteness = data.cutenessValue;
                        newMask.finalFear = data.fearValue;
                        newMask.finalColor = data.tierColor;
                        newMask.runtimeDisplayName = $"{data.rarityLabel} {newMask.maskTypeName}";
                    }

                    // Assign the setup mask to the cat
                    newCat.equippedMask = newMask;
                }

                myTeam.Add(newCat);
            }
        }
        UpdateDebugView();
    }

    private void HandleInitialVisibility()
    {
        // Always start visible so the tray populates immediately.
        // The FightManager will hide it later if needed (e.g. during Gacha spin).
        SetTrayVisibility(true);
        UpdateUI();
    }

    // --- REGISTRATION & FINDING ---

    public void RegisterSlot(TeamSlotUI slot, int index)
    {
        if (index >= 0 && index < 3)
        {
            slotUIs[index] = slot;
            
            // Immediate Refresh upon connection
            if (index < myTeam.Count && myTeam[index] != null)
                slot.Refresh(myTeam[index]);
            else
                slot.Refresh(null);
        }
    }

    private void FindTrayPanel()
    {
        Canvas mainCanvas = Object.FindAnyObjectByType<Canvas>();
        if (mainCanvas == null) return;

        Transform[] allTransforms = mainCanvas.GetComponentsInChildren<Transform>(true);
        foreach(Transform t in allTransforms)
        {
            if (t.name == "CatTeamPanel")
            {
                trayPanel = t.gameObject;
                break;
            }
        }
    }

    public void UpdateUI()
    {
        if (slotUIs != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (slotUIs[i] != null)
                {
                    if (i < myTeam.Count && myTeam[i] != null)
                        slotUIs[i].Refresh(myTeam[i]);
                    else
                        slotUIs[i].Refresh(null); 
                }
            }
        }
        UpdateDebugView();
    }

    public void SetTrayVisibility(bool isVisible)
    {
        if (trayPanel == null) FindTrayPanel();
        if (trayPanel != null) trayPanel.SetActive(isVisible);
    }

    public void EquipMaskToCat(int index, Mask mask)
    {
        if (index >= 0 && index < myTeam.Count && myTeam[index] != null)
        {
            myTeam[index].EquipMask(mask);
            Debug.Log($"<color=cyan>TEAM MANAGER:</color> Equipped {mask.runtimeDisplayName} to {myTeam[index].catName}");
            UpdateUI(); 
        }
    }

    // --- DEBUG ---

    private void UpdateDebugView()
    {
        teamStatusView.Clear();
        for (int i = 0; i < myTeam.Count; i++)
        {
            Cat c = myTeam[i];
            string maskInfo = (c.equippedMask != null) ? c.equippedMask.runtimeDisplayName : "None";
            teamStatusView.Add($"[{i}] {c.catName} | Mask: {maskInfo} | C:{c.TotalCuteness} F:{c.TotalFear}");
        }
    }

    [ContextMenu("Debug: Equip Mask Now")]
    [ContextMenu("Debug: Equip Mask Now")]
    public void DebugEquipMask()
    {
        if (debugMaskToEquip == null)
        {
            Debug.LogWarning("Assign a mask to 'Debug Mask To Equip' first!");
            return;
        }

        Mask newMask = Instantiate(debugMaskToEquip);
        int tierIndex = (int)debugRarity;

        if (newMask.rarityTiers == null || tierIndex >= newMask.rarityTiers.Length)
        {
            tierIndex = 0;
        }

        Mask.RarityData data = newMask.rarityTiers[tierIndex];
        newMask.finalCuteness = data.cutenessValue;
        newMask.finalFear = data.fearValue;
        newMask.finalColor = data.tierColor;
        newMask.runtimeDisplayName = $"{data.rarityLabel} {newMask.maskTypeName}";

        EquipMaskToCat(debugCatIndex, newMask);

        // --- THE FIX: FORCE FIGHT MANAGER TO RESPAWN ---
        // 1. Find the FightManager (it exists in Scene 4)
        FightManager fm = Object.FindAnyObjectByType<FightManager>();
        
        // 2. If found, tell it to wipe the old cats and spawn new ones
        if (fm != null)
        {
            fm.RefreshPlayerTeam();
        }
    }

    [ContextMenu("Debug: Unequip All")]
    public void DebugUnequipAll()
    {
        foreach(var cat in myTeam) cat.equippedMask = null;
        UpdateUI();
    }
}