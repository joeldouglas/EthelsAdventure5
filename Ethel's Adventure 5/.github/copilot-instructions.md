# Copilot / AI Agent Instructions

Quick, actionable guidance to help an AI coding agent be immediately productive in this Unity project.

1) Project Big Picture
- This is a Unity (URP) game project. Gameplay is driven by scene-based singletons (managers) and UI prefabs.
- Key runtime systems: `TeamManager` (persistent player team / runtime ScriptableObject copies), `FightManager` (battle flow + UI state), `SceneTransitions` (scene load + FMOD bank handling).

2) Important files to inspect first
- [Assets/Scripts/TeamManager.cs](Assets/Scripts/TeamManager.cs) — central runtime team state, instantiates ScriptableObject `Cat`/`Mask` objects at runtime to avoid mutating assets.
- [Assets/Scripts/TeamSlotUI.cs](Assets/Scripts/TeamSlotUI.cs) — UI self-registration pattern; slots register themselves with `TeamManager.Instance.RegisterSlot(this, slotIndex)` in `Start()`.
- [Assets/Scripts/FightManager.cs](Assets/Scripts/FightManager.cs) — battle state machine, tray visibility rules, gacha integration.
- [Assets/Scripts/Fighter.cs](Assets/Scripts/Fighter.cs) — fighter prefab wiring and UI updates.
- [Assets/Scripts/SceneTransitions.cs](Assets/Scripts/SceneTransitions.cs) — FMOD integration and singletons; manages audio banks on scene swaps.
- `Assets/Scripts/ScriptableObjects/` — ScriptableObjects (Cat, Mask, etc.) used as data templates.

3) Project-specific conventions & patterns
- Singletons: Most managers expose a static `Instance` and call `DontDestroyOnLoad` (e.g., `TeamManager`, `SceneTransitions`). Expect cross-scene state to persist; managers re-find scene-local UI on `SceneManager.sceneLoaded`.
- Runtime copies of ScriptableObjects: Never mutate asset SOs at runtime. The codebase consistently `Instantiate(...)` ScriptableObjects (see `TeamManager.InitializeTeam()` and mask handling) and then mutate the instance.
- UI registration: UI prefabs self-register to managers in `Start()` instead of the manager searching the scene at Awake. Follow `TeamSlotUI` pattern for new UI components.
- Tray visibility: `TeamManager.SetTrayVisibility(bool)` is used across scenes; `FightManager` toggles the tray depending on battle/gacha state. Preserve these calls when refactoring flow.

4) External dependencies & integration points
- FMOD: `fmod/` + `SceneTransitions` use FMOD Studio API — audio banks are loaded/unloaded during scene transitions.
- LeanTween: used for simple UI/fighter animations (Assets/LeanTween).
- TextMeshPro, Unity Input System (`InputSystem_Actions.inputactions`) and URP are in use — prefer Unity editor tooling and inspector wiring.

5) Common runtime pitfalls to watch for
- Null references from inspector-missing assignments (e.g., `maskOverlay`, `fighterUIPrefab`). Check Prefabs and Scene GameObjects in the Inspector.
- Scene build index coupling: code uses numeric build indexes for scene transitions (see `SceneTransitions.TransitionTo(int)` and usages in `FightManager`). If you reorder scenes, update calls or replace with scene names.
- Editing ScriptableObject assets at runtime will corrupt data. Always `Instantiate` before mutating.

6) Developer workflows (how to build / debug)
- Open the project in the Unity Editor (use the solution `Ethel's Adventure 5.slnx` in an IDE for debugging). Attach the IDE debugger to the Unity Editor to step through MonoBehaviours.
- Quick CLI build example (replace `<version>` and paths):

  /Applications/Unity/Hub/Editor/<version>/Unity -projectPath "/path/to/project" -quit -batchmode -buildWindows64Player "Builds/Win/Ethels.exe"

- Use the Unity Editor Play mode for scene-specific testing; use `Debug.Log` and existing `ContextMenu` debug helpers (e.g., `TeamManager.DebugEquipMask`) for manual tests.

7) How to add or change game data
- Add new `Cat` or `Mask` ScriptableObjects under `Assets/Scripts/ScriptableObjects/` and assign them in inspectors (Gacha, Enemy archetypes, `TeamManager.startingCats`).

8) Micro-examples (patterns to replicate)
- Instantiate SO before mutating (from `TeamManager`):

  ```csharp
  Cat newCat = Instantiate(startingCats[i]);
  Mask newMask = Instantiate(newCat.equippedMask);
  // then set runtime fields (finalCuteness, runtimeDisplayName, etc.)
  ```

- UI self-registration (from `TeamSlotUI`): `TeamManager.Instance.RegisterSlot(this, slotIndex);`

9) Notes & small gotchas
- There is a misspelled class (`GameManageer`) in `Assets/Scripts/GameManager.cs`; be careful when renaming or referencing this file.
- Many behaviors rely on inspector wiring — prefer editing scenes/prefabs in the Editor instead of creating references purely in code.

If anything above is unclear or you want the guide expanded with build scripts or CI examples, tell me which area to expand.
