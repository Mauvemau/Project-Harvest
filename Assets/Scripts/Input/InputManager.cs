using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    [Header("Player Actions")]
    [SerializeField] private InputActionReference playerMoveAction;
    [SerializeField] private InputActionReference playerAimAction;
    [SerializeField] private InputActionReference playerAimContinuousAction;
    [SerializeField] private InputActionReference playerInteractAction;

    [Header("UI Actions")]
    [SerializeField] private InputActionReference uiSubmitAction;
    [SerializeField] private InputActionReference uiCancelAction;
    [SerializeField] private InputActionReference uiPauseAction;

    [Header("Debug Actions")] 
    [SerializeField] private InputActionReference debugComboPrefixAction;
    [SerializeField] private InputActionReference debugLevelUpAction;
    [SerializeField] private InputActionReference debugInstantDeathAction;
    [SerializeField] private InputActionReference debugInstantWinAction;

    [Header("Settings")] 
    [SerializeField, Range(0f, 1f)] private float stickDeadZone = .2f;

    public static event Action<Vector2> OnPlayerMoveInputPerformed = delegate {};
    public static event Action<Vector2> OnPlayerAimInputPerformed = delegate {};
    public static event Action OnPlayerInteractInputPerformed = delegate {};
    
    public static event Action OnUISubmitInputStarted = delegate {};
    public static event Action OnUISubmitInputCancelled = delegate {};
    public static event Action OnUICancelInputStarted = delegate {};
    public static event Action OnUICancelInputCancelled = delegate {};
    public static event Action OnUIPauseInputStarted = delegate {};
    
    public static event Action OnDebugLevelUpInputPerformed = delegate {};
    public static event Action OnDebugInstantDeathInputPerformed = delegate {};
    public static event Action OnDebugInstantWinInputPerformed = delegate {};


    private bool _shouldReadPlayerInput = false;
    private bool _shouldReadMouseInput = true;
    private bool _shouldReadDebugInput = false;

    // Public

    public void SetPlayerInputEnabled(bool shouldReadPlayerInput) {
        _shouldReadPlayerInput = shouldReadPlayerInput;
    }

    // Tools
    
    private Vector2 ApplyDeadZone(Vector2 input) {
        if (input.magnitude < stickDeadZone) {
            return Vector2.zero;
        }

        return input.normalized * ((input.magnitude - stickDeadZone) / (1 - stickDeadZone));
    }

    private bool IsGamePaused() {
        return Time.timeScale <= 0f;
    }
    
    private bool ShouldReadPlayerInput() {
        return _shouldReadPlayerInput;
    }
    
    // Input Handler Functions

    // Player
    
    private void HandlePlayerMoveInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadPlayerInput()) return;
        OnPlayerMoveInputPerformed?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void HandlePlayerAimInputMouse(InputAction.CallbackContext ctx) {
        if (!_shouldReadMouseInput) return;
        if (IsGamePaused()) return;
        if (!ShouldReadPlayerInput()) return;
        
        OnPlayerAimInputPerformed?.Invoke(ctx.ReadValue<Vector2>());
    }
    
    private void HandlePlayerAimInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadPlayerInput()) return;
        _shouldReadMouseInput = !ctx.action.inProgress;
        if (IsGamePaused()) return;
        
        Vector2 direction = ApplyDeadZone(ctx.ReadValue<Vector2>());
        
        OnPlayerAimInputPerformed?.Invoke(direction);
    }

    private void HandlePlayerInteractInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadPlayerInput()) return;
        OnPlayerInteractInputPerformed?.Invoke();
    }

    // UI
    private void HandleSubmitInput(InputAction.CallbackContext ctx) {
        if (ctx.started) {
            OnUISubmitInputStarted?.Invoke();
        }
        if (ctx.canceled) {
            OnUISubmitInputCancelled?.Invoke();
        }
    }

    private void HandleCancelInput(InputAction.CallbackContext ctx) {
        if (ctx.started) {
            OnUICancelInputStarted?.Invoke();
        }
        if (ctx.canceled) {
            OnUICancelInputCancelled?.Invoke();
        }
    }

    private void HandlePauseInput(InputAction.CallbackContext ctx) {
        OnUIPauseInputStarted?.Invoke();
    }
    
    // Debug

    private void HandleDebugPrefixInput(InputAction.CallbackContext ctx) {
        _shouldReadDebugInput = ctx.started;
    }

    private void HandleLevelUpInput(InputAction.CallbackContext ctx) {
        if (!_shouldReadDebugInput || !_shouldReadPlayerInput || IsGamePaused()) return;
        if (!Debug.isDebugBuild) return;
        OnDebugLevelUpInputPerformed?.Invoke();
    }
    
    private void HandleInstantDeathInput(InputAction.CallbackContext ctx) {
        if (!_shouldReadDebugInput || !_shouldReadPlayerInput || IsGamePaused()) return;
        if (!Debug.isDebugBuild) return;
        OnDebugInstantDeathInputPerformed?.Invoke();
    }

    private void HandleInstantWinInput(InputAction.CallbackContext ctx) {
        if (!_shouldReadDebugInput || !_shouldReadPlayerInput || IsGamePaused()) return;
        if (!Debug.isDebugBuild) return;
        OnDebugInstantWinInputPerformed?.Invoke();
    }

    // Input Events

    private void OnEnable() {
        // Player Actions
        if (playerMoveAction) {
            playerMoveAction.action.started += HandlePlayerMoveInput;
            playerMoveAction.action.performed += HandlePlayerMoveInput;
            playerMoveAction.action.canceled += HandlePlayerMoveInput;
        }
        if (playerAimAction) {
            playerAimAction.action.started += HandlePlayerAimInput;
            playerAimAction.action.performed += HandlePlayerAimInput;
            playerAimAction.action.canceled += HandlePlayerAimInput;
        }
        if (playerAimContinuousAction) {
            playerAimContinuousAction.action.performed += HandlePlayerAimInputMouse;
        }
        if (playerInteractAction) {
            playerInteractAction.action.started += HandlePlayerInteractInput;
        }
        
        // UI Actions
        if (uiSubmitAction) {
            uiSubmitAction.action.started += HandleSubmitInput;
            uiSubmitAction.action.canceled += HandleSubmitInput;
        }
        if (uiCancelAction) {
            uiCancelAction.action.started += HandleCancelInput;
            uiCancelAction.action.canceled += HandleCancelInput;
        }
        if (uiPauseAction) {
            uiPauseAction.action.started += HandlePauseInput;
        }
        
        // Debug Actions
        if (debugComboPrefixAction) {
            debugComboPrefixAction.action.started += HandleDebugPrefixInput;
            debugComboPrefixAction.action.canceled += HandleDebugPrefixInput;
        }

        if (debugLevelUpAction) {
            debugLevelUpAction.action.started += HandleLevelUpInput;
        }
        if (debugInstantDeathAction) {
            debugInstantDeathAction.action.started += HandleInstantDeathInput;
        }
        if (debugInstantWinAction) {
            debugInstantWinAction.action.started  += HandleInstantWinInput;
        }
    }

    private void OnDisable() {
        // Player Actions
        if (playerMoveAction) {
            playerMoveAction.action.started -= HandlePlayerMoveInput;
            playerMoveAction.action.performed -= HandlePlayerMoveInput;
            playerMoveAction.action.canceled -= HandlePlayerMoveInput;
        }
        if (playerAimAction) {
            playerAimAction.action.started -= HandlePlayerAimInput;
            playerAimAction.action.performed -= HandlePlayerAimInput;
            playerAimAction.action.canceled -= HandlePlayerAimInput;
        }
        if (playerAimContinuousAction) {
            playerAimContinuousAction.action.performed -= HandlePlayerAimInputMouse;
        }
        if (playerInteractAction) {
            playerInteractAction.action.started -= HandlePlayerInteractInput;
        }
        
        // UI Actions
        if (uiSubmitAction) {
            uiSubmitAction.action.started -= HandleSubmitInput;
            uiSubmitAction.action.canceled -= HandleSubmitInput;
        }
        if (uiCancelAction) {
            uiCancelAction.action.started -= HandleCancelInput;
            uiCancelAction.action.canceled -= HandleCancelInput;
        }
        if (uiPauseAction) {
            uiPauseAction.action.started -= HandlePauseInput;
        }
        
        // Debug Actions
        if (debugComboPrefixAction) {
            debugComboPrefixAction.action.started -= HandleDebugPrefixInput;
            debugComboPrefixAction.action.canceled -= HandleDebugPrefixInput;
        }

        if (debugLevelUpAction) {
            debugLevelUpAction.action.started -= HandleLevelUpInput;
        }
        if (debugInstantDeathAction) {
            debugInstantDeathAction.action.started -= HandleInstantDeathInput;
        }
        if (debugInstantWinAction) {
            debugInstantWinAction.action.started  -= HandleInstantWinInput;
        }
    }
}
