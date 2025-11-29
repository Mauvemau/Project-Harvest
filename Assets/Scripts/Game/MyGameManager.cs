using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyGameManager : MonoBehaviour {
    [Header("Gameplay Events")]
    [SerializeField] GameplayEventManager gameplayEventManager;

    [Header("Weapon Upgrades Manager")]
    [SerializeField] private WeaponUpgradeManager weaponUpgradeManager;

    [Header("Global Variables")]
    [SerializeField] private GlobalVariableManager globalVariableManager;

    [Header("References")]
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private SpawnerPH spawnManager;
    [SerializeField] private InputManager inputManager;

    [Header("Game Time")]
    [SerializeField] private GameTimeManager timeManager;
    [SerializeField] private float timerPollingInterval = 1f;

    [Header("Event Invokers")]
    [SerializeField] private VoidEventChannelSO onDefeatChannel;
    [SerializeField] private VoidEventChannelSO onOpenPauseMenuChannel;
    [SerializeField] private VoidEventChannelSO onCloseAllMenusChannel;
    [SerializeField] private VoidEventChannelSO onInstantWinChannel;
    [SerializeField] private BoolEventChannelSO onToggleHudChannel;

    [Header("Event Listeners")]
    [SerializeField] private VoidEventChannelSO onStartGameChannel;
    [SerializeField] private VoidEventChannelSO onEndGameChannel;
    [SerializeField] private VoidEventChannelSO onExitAppChannel;
    
    [Header("Debug Controls")]
    [SerializeField] private bool spawnEnemies = true;
    
    private bool _gameStarted = false;
    private ITimer _currentGameTimer;
    private float _nextTimerPoll = 0f;

    public static event Action OnGameEnd = delegate {};
    public static event Action OnGameStart = delegate {};
    public static event Action<float> OnUpdateGameTimer = delegate {};

    [ContextMenu("Debug - Level Up Player")]
    private void DebugLevelUp() {
        if (!Debug.isDebugBuild) return;
        if (timeManager.IsGamePaused()) return;
        if (!playerCharacter.IsAlive()) return;
        globalVariableManager.DebugLevelUp();
    }

    private void DebugInstantWin() {
        if (!Debug.isDebugBuild) return;
        if (timeManager.IsGamePaused()) return;
        if (!playerCharacter.IsAlive()) return;
        onInstantWinChannel?.RaiseEvent();
    }
    
    /// <summary>
    /// Used for debugging gameplay events
    /// </summary>
    public void DebugTimestampMessage(string message) {
        Debug.Log($"{name}: " + message);
    }
    
    private void TogglePause() {
        if (!_gameStarted) return;
        if (!playerCharacter.IsAlive()) return;
        if (!timeManager.IsGamePaused()) {
            onOpenPauseMenuChannel?.RaiseEvent();
        }
        else {
            // onCloseAllMenusChannel?.RaiseEvent();
        }
    }
    
    private void SetHudEnabled(bool shouldDrawHud) {
        if(onToggleHudChannel) {
            onToggleHudChannel?.RaiseEvent(shouldDrawHud);
        }
    }

    private void HandlePlayerDeath() {
        if (!_gameStarted) return;
        if (onDefeatChannel) {
            onDefeatChannel?.RaiseEvent();
        }
    }

    private void ExitApplication() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    [ContextMenu("Debug - End Game")]
    private void EndGame() {
        _gameStarted = false;
        _currentGameTimer.Stop();
        spawnManager.SetSpawning(false);
        spawnManager.Wipe();
        globalVariableManager.ResetAll();
        weaponUpgradeManager.UnequipAll();
        OnGameEnd?.Invoke();
        inputManager.SetPlayerInputEnabled(false);
        playerCharacter.gameObject.SetActive(false);
        SetHudEnabled(false);
    }
    
    [ContextMenu("Debug - Start Game")]
    private void StartGame() {
        gameplayEventManager.Reset();
        weaponUpgradeManager.Init();
        playerCharacter.gameObject.SetActive(true);
        inputManager.SetPlayerInputEnabled(true);
        SetHudEnabled(true);
        if (spawnEnemies) {
            spawnManager.SetSpawning(true);
        }
        _currentGameTimer.Start();
        _nextTimerPoll = 0;
        _gameStarted = true;
        OnGameStart?.Invoke();
    }

    private void Update() {
        timeManager.Update();
        
        if (timeManager.IsGamePaused()) return;
        gameplayEventManager.Update(_currentGameTimer.CurrentTime, !spawnEnemies);

        if (Time.time < _nextTimerPoll) return;
        _nextTimerPoll = Time.time + timerPollingInterval;
        OnUpdateGameTimer?.Invoke(_currentGameTimer.CurrentTime);
    }

    private void Awake() {
        Random.InitState(System.DateTime.Now.Millisecond);
        globalVariableManager.Init();
        _currentGameTimer = timeManager.CreateTimer();
    }
    
    private void OnValidate() {
        gameplayEventManager.OnValidate();
    }

    private void OnEnable() {
        timeManager.OnEnable();
        weaponUpgradeManager.OnEnable();
        
        PlayerCharacter.OnPlayerDeath += HandlePlayerDeath;
        ExperienceCollectible.OnExperienceCollected += globalVariableManager.AddCurrentExperience;
        InputManager.OnDebugLevelUpInputPerformed += DebugLevelUp;
        InputManager.OnUIPauseInputStarted += TogglePause;
        InputManager.OnDebugInstantWinInputPerformed += DebugInstantWin;
        
        if (onStartGameChannel) {
            onStartGameChannel.OnEventRaised += StartGame;
        }
        if (onEndGameChannel) {
            onEndGameChannel.OnEventRaised += EndGame;
        }
        if (onExitAppChannel) {
            onExitAppChannel.OnEventRaised += ExitApplication;
        }
    }
    
    private void OnDisable() {
        timeManager.OnDisable();
        weaponUpgradeManager.OnDisable();
        
        PlayerCharacter.OnPlayerDeath -= HandlePlayerDeath;
        ExperienceCollectible.OnExperienceCollected -= globalVariableManager.AddCurrentExperience;
        InputManager.OnDebugLevelUpInputPerformed -= DebugLevelUp;
        InputManager.OnUIPauseInputStarted -= TogglePause;
        InputManager.OnDebugInstantWinInputPerformed -= DebugInstantWin;
        
        if (onStartGameChannel) {
            onStartGameChannel.OnEventRaised -= StartGame;
        }
        if (onEndGameChannel) {
            onEndGameChannel.OnEventRaised -= EndGame;
        }
        if (onExitAppChannel) {
            onExitAppChannel.OnEventRaised -= ExitApplication;
        }
    }
}
