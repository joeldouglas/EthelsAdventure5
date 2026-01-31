using UnityEngine;
using System.Collections.Generic;
public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    [Header("Team Setup")]
    // Drag your Archetype Files (Assets) here in the Inspector
    [SerializeField] private List<Cat> startingCats; 
    
    // This list holds the LIVE versions (Instances)
    [HideInInspector] public List<Cat> myTeam = new List<Cat>(); 

    [Header("UI References")]
    public TeamSlotUI[] slotUIs; 

    void Awake()
    {
        Instance = this;
        InitializeTeam();
    }

    private void InitializeTeam()
    {
        myTeam.Clear();
        for (int i = 0; i < startingCats.Count; i++)
        {
            if (startingCats[i] != null)
            {
                // Create a live instance so we don't change the original file
                Cat liveCat = Instantiate(startingCats[i]);
                myTeam.Add(liveCat);
            }
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < myTeam.Count; i++)
        {
            if (i < slotUIs.Length)
            {
                // We pass the LIVE instance to the UI slot
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