using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using TMPro; 
using UnityEngine.UI;

public class FightManager : MonoBehaviour
{
    public enum BattleState { WaitingToStart, Battling, VictoryPrompt, GachaActive, EndScreen }
    public BattleState currentState = BattleState.WaitingToStart;

    [Header("UI References")]
    public GameObject promptPanel;     
    public TextMeshProUGUI promptText; 
    public GameObject battleCanvas;    
    
    [Header("References")]
    public GachaBehaviour gacha;       

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
        currentState = BattleState.WaitingToStart;
        UpdatePrompt("Press Space to Start Battle");
        
        // Ensure Tray is visible
        if(TeamManager.Instance != null)
        {
            TeamManager.Instance.SetTrayVisibility(true);
            TeamManager.Instance.UpdateUI(); 
        }

        // Setup Enemies
        List<Cat> randomEnemies = new List<Cat>(enemyArchetypes);
        ShuffleCats(randomEnemies);
        LoadTeams(randomEnemies);
    }

    // --- NEW: REFRESH METHOD FOR DEBUGGING ---
    // Called by TeamManager when you equip a mask via Debug
    public void RefreshPlayerTeam()
    {
        // 1. Destroy existing player objects
        foreach (var fighter in playerFighters)
        {
            if (fighter != null) Destroy(fighter.gameObject);
        }
        playerFighters.Clear();

        // 2. Respawn using latest TeamManager data
        if (TeamManager.Instance != null)
        {
            List<Cat> playerCats = TeamManager.Instance.myTeam;
            for (int i = 0; i < playerCats.Count; i++)
            {
                if (playerCats[i] != null && i < playerSlots.Length)
                    SpawnFighter(playerCats[i], playerSlots[i], true);
            }
        }
        
        Debug.Log("FIGHT MANAGER: Player team refreshed with new stats/masks!");
    }

    // --- INPUT HANDLER ---
    public void OnSpacePressed()
    {
        if (currentState == BattleState.WaitingToStart)
        {
            StartBattle();
        }
        else if (currentState == BattleState.VictoryPrompt)
        {
            StartGachaSpin();
        }
        else if (currentState == BattleState.EndScreen)
        {
            SceneTransitions.Instance.TransitionTo(3);
        }
    }

    public void StartBattle()
    {
        currentState = BattleState.Battling;
        if(promptPanel != null) promptPanel.SetActive(false);
        StartCoroutine(BattleRoutine());
    }

    public void StartGachaSpin()
    {
        currentState = BattleState.GachaActive;
        if(promptPanel != null) promptPanel.SetActive(false);
        // TeamManager.Instance.SetTrayVisibility(false); // Hide tray for spin
        if (gacha != null) gacha.SpinSlotMachine();
    }

    private IEnumerator BattleRoutine()
    {
        while (playerFighters.Count > 0 && enemyFighters.Count > 0)
        {
            for (int lane = 0; lane < 3; lane++)
            {
                if (playerFighters.Count == 0 || enemyFighters.Count == 0) break;

                // Player Turn
                Fighter playerCat = GetFighterAtSlot(playerSlots, lane);
                if (playerCat != null)
                {
                    Fighter target = FindTarget(true, lane); 
                    if (target != null) yield return ExecuteAttack(playerCat, target);
                }
                yield return new WaitForSeconds(0.2f);

                // Enemy Turn
                Fighter enemyCat = GetFighterAtSlot(enemySlots, lane);
                if (enemyCat != null)
                {
                    Fighter target = FindTarget(false, lane);
                    if (target != null) yield return ExecuteAttack(enemyCat, target);
                }
                yield return new WaitForSeconds(timeBetweenTurns);
            }
        }

        // --- BATTLE ENDED ---
        bool playerWon = playerFighters.Count > 0;
        
        if (playerWon)
        {
            currentState = BattleState.VictoryPrompt;
            if(battleCanvas != null) battleCanvas.SetActive(false); 
            if(gacha != null) gacha.InitializeGacha();
            if(TeamManager.Instance != null) TeamManager.Instance.SetTrayVisibility(false); 
            UpdatePrompt("Victory! Press Space to Spin");
        }
        else
        {
            ShowEndScreen("Defeat... Press Space to Leave");
        }
    }

    public void OnGachaCompleted()
    {
        ShowEndScreen("Finished! Press Space to Leave Casino");
    }

    private void ShowEndScreen(string message)
    {
        currentState = BattleState.EndScreen;
        UpdatePrompt(message);
        TeamManager.Instance.SetTrayVisibility(true);
        TeamManager.Instance.UpdateUI();
    }

    void UpdatePrompt(string message)
    {
        if(promptText != null) promptText.text = message;
        if(promptPanel != null) promptPanel.SetActive(true);
    }

    void ShuffleCats(List<Cat> listToShuffle)
    {
        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Cat temp = listToShuffle[i];
            listToShuffle[i] = listToShuffle[randomIndex];
            listToShuffle[randomIndex] = temp;
        }
    }

    void LoadTeams(List<Cat> enemiesToSpawn)
    {
        playerFighters.Clear();
        enemyFighters.Clear();

        List<Cat> playerCats = TeamManager.Instance.myTeam;
        for (int i = 0; i < playerCats.Count; i++)
        {
            if (playerCats[i] != null && i < playerSlots.Length)
                SpawnFighter(playerCats[i], playerSlots[i], true);
        }

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            if (enemiesToSpawn[i] != null && i < enemySlots.Length)
            {
                Cat enemyInstance = Instantiate(enemiesToSpawn[i]);
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

    private Fighter GetFighterAtSlot(RectTransform[] slots, int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index].GetComponentInChildren<Fighter>();
    }

    private Fighter FindTarget(bool attackerIsPlayer, int lane)
    {
        RectTransform[] opponentSlots = attackerIsPlayer ? enemySlots : playerSlots;
        Fighter target = GetFighterAtSlot(opponentSlots, lane);
        if (target != null) return target;

        if (lane == 1) 
        {
            target = GetFighterAtSlot(opponentSlots, 0);
            if (target == null) target = GetFighterAtSlot(opponentSlots, 2);
        }
        else 
        {
            target = GetFighterAtSlot(opponentSlots, 1);
        }
        
        if (target == null && lane == 0) target = GetFighterAtSlot(opponentSlots, 2);
        if (target == null && lane == 2) target = GetFighterAtSlot(opponentSlots, 0);

        return target;
    }

    private IEnumerator ExecuteAttack(Fighter attacker, Fighter defender)
    {
        Vector3 originalPos = attacker.transform.localPosition;
        Vector3 targetDir = (defender.transform.position - attacker.transform.position).normalized * 50f;
        
        LeanTween.moveLocal(attacker.gameObject, originalPos + targetDir, 0.2f).setEaseOutQuad();
        yield return new WaitForSeconds(0.2f);

        int damageToDef = attacker.currentCuteness;
        int damageToAtk = defender.currentCuteness;

        defender.currentFear -= damageToDef;
        attacker.currentFear -= damageToAtk;

        if (defender.currentFear < 0) defender.currentFear = 0;
        if (attacker.currentFear < 0) attacker.currentFear = 0;

        defender.UpdateUI();
        attacker.UpdateUI();

        float defX = defender.transform.localPosition.x;
        LeanTween.moveLocalX(defender.gameObject, defX + 10f, 0.1f).setLoopPingPong(2);

        float atkX = attacker.transform.localPosition.x;
        LeanTween.moveLocalX(attacker.gameObject, atkX - 10f, 0.1f).setLoopPingPong(2);

        bool attackerDied = false;

        if (defender.currentFear <= 0) HandleDefeat(defender);
        if (attacker.currentFear <= 0) 
        {
            HandleDefeat(attacker);
            attackerDied = true;
        }

        if (!attackerDied)
        {
            LeanTween.moveLocal(attacker.gameObject, originalPos, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
        else yield return new WaitForSeconds(0.2f);
    }

    private void HandleDefeat(Fighter f)
    {
        if (f.isPlayer) playerFighters.Remove(f);
        else enemyFighters.Remove(f);
        Destroy(f.gameObject);
    }
}