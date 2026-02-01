// using UnityEngine;
// using System.Collections.Generic;

// public class FightManager : MonoBehaviour
// {
//     [Header("Spawn Locations (UI Rects)")]
//     public RectTransform[] playerSlots; 
//     public RectTransform[] enemySlots;

//     [Header("Prefabs")]
//     public GameObject fighterUIPrefab; 
//     public GameObject OppUIPrefab; 

//     [Header("Enemy Setup")]
//     public List<Cat> enemyArchetypes; 
//     public float difficultyMultiplier = 1.2f;

//     private List<Fighter> playerFighters = new List<Fighter>();
//     private List<Fighter> enemyFighters = new List<Fighter>();

//     [Header("External References")]
//     public GachaBehaviour gacha; // Drag Gacha object here


//     void Start()
//     {
//         // 1. Shuffle the enemy slots so we don't know who goes where
//         ShuffleSlots(enemySlots);
        
//         // 2. Load the teams into the (now randomized) slots
//         LoadTeams();
//     }

//     void ShuffleSlots(RectTransform[] slotsToShuffle)
//     {
//         // Fisher-Yates Shuffle Algorithm
//         for (int i = slotsToShuffle.Length - 1; i > 0; i--)
//         {
//             int randomIndex = Random.Range(0, i + 1);
            
//             // Swap the references in the array
//             RectTransform temp = slotsToShuffle[i];
//             slotsToShuffle[i] = slotsToShuffle[randomIndex];
//             slotsToShuffle[randomIndex] = temp;
//         }
//     }

//     void LoadTeams()
//     {
//         // Player Loading (Stays in order 0, 1, 2 for player consistency)
//         List<Cat> playerCats = TeamManager.Instance.myTeam;
//         for (int i = 0; i < playerCats.Count; i++)
//         {
//             if (playerCats[i] != null && i < playerSlots.Length)
//             {
//                 SpawnFighter(playerCats[i], playerSlots[i], true);
//             }
//         }

//         // Enemy Loading (Uses the shuffled enemySlots array)
//         for (int i = 0; i < enemyArchetypes.Count; i++)
//         {
//             if (enemyArchetypes[i] != null && i < enemySlots.Length)
//             {
//                 Cat enemyInstance = Instantiate(enemyArchetypes[i]);
//                 enemyInstance.baseCuteness = Mathf.RoundToInt(enemyInstance.baseCuteness * difficultyMultiplier);
//                 enemyInstance.baseFear = Mathf.RoundToInt(enemyInstance.baseFear * difficultyMultiplier);

//                 // This 'enemySlots[i]' is now a random slot from the hierarchy!
//                 SpawnFighter(enemyInstance, enemySlots[i], false);
//             }
//         }
//     }

//     void SpawnFighter(Cat data, RectTransform parentSlot, bool isPlayerSide)
//     {
//         GameObject prefabToUse = isPlayerSide ? fighterUIPrefab : OppUIPrefab;
//         GameObject go = Instantiate(prefabToUse, parentSlot);
        
//         RectTransform rt = go.GetComponent<RectTransform>();
//         rt.anchoredPosition = Vector2.zero;
//         rt.localPosition = Vector3.zero;

//         Fighter fighter = go.GetComponent<Fighter>();
//         fighter.Initialize(data, isPlayerSide);
        
//         if (isPlayerSide) playerFighters.Add(fighter);
//         else enemyFighters.Add(fighter);
//     }

//     [Header("Battle Settings")]
// public float timeBetweenTurns = 1.0f;

// public void StartBattle()
// {
//     StartCoroutine(BattleRoutine());
// }

// private IEnumerator BattleRoutine()
// {
//     // We continue as long as both sides have at least one cat standing
//     while (playerFighters.Count > 0 && enemyFighters.Count > 0)
//     {
//         for (int lane = 0; lane < 3; lane++)
//         {
//             // Verify there are still fighters left before starting a lane turn
//             if (playerFighters.Count == 0 || enemyFighters.Count == 0) break;

//             // --- PLAYER TURN FOR THIS LANE ---
//             Fighter playerCat = GetFighterInLane(playerFighters, lane);
//             if (playerCat != null)
//             {
//                 Fighter target = FindTarget(enemyFighters, lane);
//                 if (target != null) yield return ExecuteAttack(playerCat, target);
//             }

//             yield return new WaitForSeconds(0.2f);

//             // --- ENEMY TURN FOR THIS LANE ---
//             Fighter enemyCat = GetFighterInLane(enemyFighters, lane);
//             if (enemyCat != null)
//             {
//                 Fighter target = FindTarget(playerFighters, lane);
//                 if (target != null) yield return ExecuteAttack(enemyCat, target);
//             }

//             yield return new WaitForSeconds(timeBetweenTurns);
//         }
//     }

//     Debug.Log("Battle Over!");
    
//     bool playerWon = playerFighters.Count > 0;
//     yield return new WaitForSeconds(1.0f);

//     if (playerWon)
//     {
//         Debug.Log("VICTORY: Press Space to head to the Slot Machine!");
//         // Wait for Spacebar to initialize Gacha
//         while (!UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame) yield return null;
        
//         gacha.InitializeGacha();
//         // The Gacha is now active. Once the player picks a cat or trashes, 
//         // they handle the exit through Gacha's existing cleanup.
//     }
//     else
//     {
//         Debug.Log("DEFEAT: Press Space to retreat...");
//         while (!UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame) yield return null;
        
//         SceneTransitions.Instance.TransitionTo(3); // Scene 3 as requested
//     }

// }

// private Fighter GetFighterInLane(List<Fighter> list, int lane)
// {
//     // We check the 'parentSlot' index to see which lane the fighter belongs to
//     // This works because we assigned playerSlots[lane] during Spawn
//     return list.Find(f => f.transform.parent == playerSlots[lane] || f.transform.parent == enemySlots[lane]);
// }

// private Fighter FindTarget(List<Fighter> opponents, int lane)
// {
//     // 1. Try direct opposite
//     Fighter target = GetFighterInLane(opponents, lane);
//     if (target != null) return target;

//     // 2. Try adjacent (Lane 1 checks 0 then 2; Lane 0 checks 1; Lane 2 checks 1)
//     if (lane == 1)
//     {
//         target = GetFighterInLane(opponents, 0);
//         if (target == null) target = GetFighterInLane(opponents, 2);
//     }
//     else
//     {
//         target = GetFighterInLane(opponents, 1);
//     }

//     return target;
// }

// private IEnumerator ExecuteAttack(Fighter attacker, Fighter defender)
// {
//     // 1. Animation: Bump towards target
//     Vector3 originalPos = attacker.transform.localPosition;
//     Vector3 targetDir = (defender.transform.position - attacker.transform.position).normalized * 50f;
    
//     LeanTween.moveLocal(attacker.gameObject, originalPos + targetDir, 0.2f).setEaseOutQuad();
//     yield return new WaitForSeconds(0.2f);

//     // 2. Damage Logic: Cuteness reduces Fear
//     defender.currentFear -= attacker.currentCuteness;
//     defender.UpdateUI();
    
//     // Shake the defender
//     LeanTween.moveMargin(defender.gameObject, 10f, 0.1f).setLoopPingPong(2);

//     // 3. Check for Defeat
//     if (defender.currentFear <= 0)
//     {
//         HandleDefeat(defender);
//     }

//     // 4. Return to original position
//     LeanTween.moveLocal(attacker.gameObject, originalPos, 0.2f);
//     yield return new WaitForSeconds(0.2f);
// }

// private void HandleDefeat(Fighter f)
// {
//     if (f.isPlayer) playerFighters.Remove(f);
//     else enemyFighters.Remove(f);

//     // Optional: Add a "Poof" particle effect here
//     Destroy(f.gameObject);
// }
// }

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using TMPro; 
using UnityEngine.UI;

public class FightManager : MonoBehaviour
{
    public enum BattleState { WaitingToStart, Battling, GachaActive, EndScreen }
    public BattleState currentState = BattleState.WaitingToStart;

    [Header("UI References")]
    public GameObject promptPanel;     // Panel holding the text background
    public TextMeshProUGUI promptText; // The text object itself
    public GameObject battleCanvas;    // Parent object of the fighter slots (to hide them during Gacha)
    
    [Header("References")]
    public GachaBehaviour gacha;       // Drag the Gacha object from the scene here

    [Header("Spawn Locations (UI Rects)")]
    public RectTransform[] playerSlots; 
    public RectTransform[] enemySlots;

    [Header("Prefabs")]
    public GameObject fighterUIPrefab; 
    public GameObject OppUIPrefab; 

    [Header("Enemy Setup")]
    public List<Cat> enemyArchetypes; 
    public float difficultyMultiplier = 1.2f;

    [Header("Battle Settings")]
    public float timeBetweenTurns = 1.0f;

    private List<Fighter> playerFighters = new List<Fighter>();
    private List<Fighter> enemyFighters = new List<Fighter>();

    void Start()
    {
        // 1. Initialize State
        currentState = BattleState.WaitingToStart;
        
        // 2. Setup UI
        if(promptPanel != null) promptPanel.SetActive(true);
        if(promptText != null) promptText.text = "Press Space to Start Battle";
        
        // Ensure the Team Tray is visible so we can see our cats
        if(TeamManager.Instance != null) TeamManager.Instance.SetTrayVisibility(true); 

        // 3. Setup Battle
        ShuffleSlots(enemySlots);
        LoadTeams();
    }

    // --- INPUT HANDLER (Called by PlayerController) ---
    public void OnSpacePressed()
    {
        if (currentState == BattleState.WaitingToStart)
        {
            StartBattle();
        }
        else if (currentState == BattleState.EndScreen)
        {
            // Leave the Casino - Go back to Scene 3 (as requested)
            Debug.Log("Leaving Catsino...");
            SceneTransitions.Instance.TransitionTo(3);
        }
    }

    // --- SETUP LOGIC ---
    void ShuffleSlots(RectTransform[] slotsToShuffle)
    {
        for (int i = slotsToShuffle.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            RectTransform temp = slotsToShuffle[i];
            slotsToShuffle[i] = slotsToShuffle[randomIndex];
            slotsToShuffle[randomIndex] = temp;
        }
    }

    void LoadTeams()
    {
        // Clear old lists just in case
        playerFighters.Clear();
        enemyFighters.Clear();

        List<Cat> playerCats = TeamManager.Instance.myTeam;
        for (int i = 0; i < playerCats.Count; i++)
        {
            if (playerCats[i] != null && i < playerSlots.Length)
                SpawnFighter(playerCats[i], playerSlots[i], true);
        }

        for (int i = 0; i < enemyArchetypes.Count; i++)
        {
            if (enemyArchetypes[i] != null && i < enemySlots.Length)
            {
                Cat enemyInstance = Instantiate(enemyArchetypes[i]);
                enemyInstance.baseCuteness = Mathf.RoundToInt(enemyInstance.baseCuteness * difficultyMultiplier);
                enemyInstance.baseFear = Mathf.RoundToInt(enemyInstance.baseFear * difficultyMultiplier);
                SpawnFighter(enemyInstance, enemySlots[i], false);
            }
        }
    }

    void SpawnFighter(Cat data, RectTransform parentSlot, bool isPlayerSide)
    {
        GameObject prefabToUse = isPlayerSide ? fighterUIPrefab : OppUIPrefab;
        GameObject go = Instantiate(prefabToUse, parentSlot);
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localPosition = Vector3.zero;

        Fighter fighter = go.GetComponent<Fighter>();
        fighter.Initialize(data, isPlayerSide);
        
        if (isPlayerSide) playerFighters.Add(fighter);
        else enemyFighters.Add(fighter);
    }

    // --- BATTLE LOGIC ---
    public void StartBattle()
    {
        currentState = BattleState.Battling;
        if(promptPanel != null) promptPanel.SetActive(false); // Hide the prompt
        
        // Hide the tray so it doesn't cover the fight
        TeamManager.Instance.SetTrayVisibility(false); 
        
        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        // Battle Loop
        while (playerFighters.Count > 0 && enemyFighters.Count > 0)
        {
            for (int lane = 0; lane < 3; lane++)
            {
                if (playerFighters.Count == 0 || enemyFighters.Count == 0) break;

                // Player Turn
                Fighter playerCat = GetFighterInLane(playerFighters, lane);
                if (playerCat != null)
                {
                    Fighter target = FindTarget(enemyFighters, lane);
                    if (target != null) yield return ExecuteAttack(playerCat, target);
                }
                yield return new WaitForSeconds(0.2f);

                // Enemy Turn
                Fighter enemyCat = GetFighterInLane(enemyFighters, lane);
                if (enemyCat != null)
                {
                    Fighter target = FindTarget(playerFighters, lane);
                    if (target != null) yield return ExecuteAttack(enemyCat, target);
                }
                yield return new WaitForSeconds(timeBetweenTurns);
            }
        }

        // --- BATTLE ENDED ---
        bool playerWon = playerFighters.Count > 0;
        
        if (playerWon)
        {
            Debug.Log("Victory! Starting Gacha.");
            currentState = BattleState.GachaActive;
            
            // Hide Battle Elements
            if(battleCanvas != null) battleCanvas.SetActive(false); 
            
            // Start Gacha
            if(gacha != null) gacha.InitializeGacha();
            else Debug.LogError("Gacha Reference Missing in FightManager!");
        }
        else
        {
            Debug.Log("Defeat.");
            ShowEndScreen("Defeat... Press Space to Leave");
        }
    }

    // --- CALLBACKS ---
    // Called by GachaBehaviour when the player is done (kept or trashed mask)
    public void OnGachaCompleted()
    {
        ShowEndScreen("Finished! Press Space to Leave Casino");
    }

    private void ShowEndScreen(string message)
    {
        currentState = BattleState.EndScreen;
        if(promptText != null) promptText.text = message;
        if(promptPanel != null) promptPanel.SetActive(true);
        
        // Bring back the tray so we can see our team before leaving
        TeamManager.Instance.SetTrayVisibility(true);
    }

    // --- HELPER FUNCTIONS ---
    private Fighter GetFighterInLane(List<Fighter> list, int lane)
    {
        // Matches the fighter to the slot parent
        return list.Find(f => f.transform.parent == playerSlots[lane] || f.transform.parent == enemySlots[lane]);
    }

    private Fighter FindTarget(List<Fighter> opponents, int lane)
    {
        // 1. Try Direct Opposite
        Fighter target = GetFighterInLane(opponents, lane);
        if (target != null) return target;

        // 2. Try Adjacent
        if (lane == 1) // Middle Lane checks Left then Right
        {
            target = GetFighterInLane(opponents, 0);
            if (target == null) target = GetFighterInLane(opponents, 2);
        }
        else // Side lanes check Middle
        {
            target = GetFighterInLane(opponents, 1);
        }
        return target;
    }

    private IEnumerator ExecuteAttack(Fighter attacker, Fighter defender)
    {
        Vector3 originalPos = attacker.transform.localPosition;
        Vector3 targetDir = (defender.transform.position - attacker.transform.position).normalized * 50f;
        
        // 1. Animation: Bump towards target
        LeanTween.moveLocal(attacker.gameObject, originalPos + targetDir, 0.2f).setEaseOutQuad();
        yield return new WaitForSeconds(0.2f);

        // 2. Logic: Damage
        defender.currentFear -= attacker.currentCuteness;
        if(defender.currentFear < 0) defender.currentFear = 0;
        defender.UpdateUI();
        
        // --- FIX IS HERE ---
        // We use moveLocalX instead of moveMargin. 
        // We move it +10 pixels to the right, then ping-pong back to create a shake.
        float shakeAmt = 10f;
        float currentX = defender.transform.localPosition.x;
        
        LeanTween.moveLocalX(defender.gameObject, currentX + shakeAmt, 0.1f).setLoopPingPong(2);
        // -------------------

        // 3. Check Death
        if (defender.currentFear <= 0) HandleDefeat(defender);

        // 4. Animation: Return Attacker
        LeanTween.moveLocal(attacker.gameObject, originalPos, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }
    private void HandleDefeat(Fighter f)
    {
        if (f.isPlayer) playerFighters.Remove(f);
        else enemyFighters.Remove(f);
        Destroy(f.gameObject);
    }
}