using UnityEngine;

public class MusicManager : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private AK.Wwise.Event startAudioEvent;

    [Header("States")] 
    [SerializeField] private AK.Wwise.State inMainMenuState;
    [SerializeField] private AK.Wwise.State inCombatState;
    [SerializeField] private AK.Wwise.State inPausedGameState;
    [SerializeField] private AK.Wwise.State victoryGameState;
    [SerializeField] private AK.Wwise.State defeatGameState;

    [Header("Settings")] 
    [SerializeField] private bool autoplayOnStart = false;
    
    [Header("Event Listeners")]
    [SerializeField] private BoolEventChannelSO onSetGamePaused;
    
    private System.Action _onGameStartHandler;
    private System.Action _onGameEndHandler;
    private System.Action _onPlayerDeathHandler;
    
    private void ChangeState(AK.Wwise.State newState) {
        newState?.SetValue();
    }
    
    private void HandleGamePaused(bool isPaused) {
        ChangeState(isPaused ? inPausedGameState : inCombatState);
    }
    
    private void Start() {
        if (!autoplayOnStart) return;
        ChangeState(inMainMenuState);
        startAudioEvent.Post(gameObject);
    }

    private void OnEnable() {
        _onGameStartHandler = () => ChangeState(inCombatState);
        _onGameEndHandler   = () => ChangeState(inMainMenuState);
        _onPlayerDeathHandler = () => ChangeState(defeatGameState);

        MyGameManager.OnGameStart += _onGameStartHandler;
        MyGameManager.OnGameEnd   += _onGameEndHandler;
        PlayerCharacter.OnPlayerDeath += _onPlayerDeathHandler;

        if (onSetGamePaused) {
            onSetGamePaused.OnEventRaised += HandleGamePaused;
        }
    }

    private void OnDisable() {
        MyGameManager.OnGameStart -= _onGameStartHandler;
        MyGameManager.OnGameEnd   -= _onGameEndHandler;
        PlayerCharacter.OnPlayerDeath -= _onPlayerDeathHandler;
        
        if (onSetGamePaused) {
            onSetGamePaused.OnEventRaised -= HandleGamePaused;
        }
    }
}
