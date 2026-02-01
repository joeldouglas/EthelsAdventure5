using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    [Header("Team Setup")]
    [SerializeField] private List<Cat> startingCats; 
    public List<Cat> myTeam = new List<Cat>(); 

    [Header("UI References")]
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
        slotUIs = new TeamSlotUI[3];
        trayPanel = null; // Reset reference
        FindTrayPanel();  // Aggressive search
        
        HandleInitialVisibility();
        UpdateDebugView();
    }

    private void Start()
    {
        FindTrayPanel();
        HandleInitialVisibility();
    }

    private void InitializeTeam()
    {
        myTeam.Clear();
        for (int i = 0; i < startingCats.Count; i++)
        {
            if (startingCats[i] != null) 
            {
                Cat newCat = Instantiate(startingCats[i]);
                if (newCat.equippedMask != null)
                {
                    Mask newMask = Instantiate(newCat.equippedMask);
                    if (newMask.rarityTiers != null && newMask.rarityTiers.Length > 0)
                    {
                        Mask.RarityData data = newMask.rarityTiers[0];
                        newMask.finalCuteness = data.cutenessValue;
                        newMask.finalFear = data.fearValue;
                        newMask.finalColor = data.tierColor;
                        newMask.runtimeDisplayName = $"{data.rarityLabel} {newMask.maskTypeName}";
                    }
                    newCat.equippedMask = newMask;
                }
                myTeam.Add(newCat);
            }
        }
        UpdateDebugView();
    }

    private void HandleInitialVisibility()
    {
        // Always attempt to show it first. Logic elsewhere (Gacha) will hide it if needed.
        SetTrayVisibility(true);
        UpdateUI();
    }

    // --- THE FIX: ROBUST PANEL FINDING ---
    private void FindTrayPanel()
    {
        // 1. Try simple search (works if active)
        GameObject directObj = GameObject.Find("CatTeamPanel");
        if (directObj != null)
        {
            trayPanel = directObj;
            return;
        }

        // 2. DEEP SEARCH: Look in ALL Canvases (including inactive objects)
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in allCanvases)
        {
            Transform[] allKids = c.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allKids)
            {
                if (t.name == "CatTeamPanel")
                {
                    trayPanel = t.gameObject;
                    return; // Found it!
                }
            }
        }
        
        // If we get here, we haven't found it yet, but RegisterSlot might find it via parents.
    }

    public void RegisterSlot(TeamSlotUI slot, int index)
    {
        if (index >= 0 && index < 3)
        {
            slotUIs[index] = slot;
            
            // --- FALLBACK FIX ---
            // If we still haven't found the tray, check the slot's parent!
            if (trayPanel == null && slot.transform.parent != null)
            {
                if (slot.transform.parent.name == "CatTeamPanel")
                {
                    trayPanel = slot.transform.parent.gameObject;
                }
            }

            if (index < myTeam.Count && myTeam[index] != null)
                slot.Refresh(myTeam[index]);
            else
                slot.Refresh(null);
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
        
        if (trayPanel != null)
        {
            trayPanel.SetActive(isVisible);
        }
        else
        {
            // Only warn if we are trying to SHOW it and can't find it
            if(isVisible) Debug.LogWarning("TeamManager: Trying to show tray, but 'CatTeamPanel' not found!");
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

    // --- DEBUG HELPERS ---
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
    public void DebugEquipMask()
    {
        if (debugMaskToEquip == null) return;

        Mask newMask = Instantiate(debugMaskToEquip);
        int tierIndex = (int)debugRarity;

        if (newMask.rarityTiers == null || tierIndex >= newMask.rarityTiers.Length) tierIndex = 0;

        Mask.RarityData data = newMask.rarityTiers[tierIndex];
        newMask.finalCuteness = data.cutenessValue;
        newMask.finalFear = data.fearValue;
        newMask.finalColor = data.tierColor;
        newMask.runtimeDisplayName = $"{data.rarityLabel} {newMask.maskTypeName}";

        EquipMaskToCat(debugCatIndex, newMask);

        // Refresh Battle
        FightManager fm = Object.FindAnyObjectByType<FightManager>();
        if (fm != null) fm.RefreshPlayerTeam();
    }

    [ContextMenu("Debug: Unequip All")]
    public void DebugUnequipAll()
    {
        foreach(var cat in myTeam) cat.equippedMask = null;
        UpdateUI();
    }
}