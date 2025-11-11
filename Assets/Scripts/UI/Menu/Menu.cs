using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour, IMenu {
    [Header("References")]
    [SerializeField] private EventSystem eventSystem;

    [Header("Button Settings")]
    [SerializeField] private Button initialButton;

    [Header("Event Listeners")]
    [SerializeField] public VoidEventChannelSO onRequestOpenRemotely;

    [Header("Event Invokers")]
    [Tooltip("Do not set if the menu is not supposed to pause the game")]
    [SerializeField] private BoolEventChannelSO onOpenMenuGamePauseChannel;

    [Tooltip("Do not set if the menu is not supposed to hide the game's hud")]
    [SerializeField] private BoolEventChannelSO onToggleHudChannel;

    [Header("Timing Settings")]
    [SerializeField, Min(0f)] private float pauseDelay = 0f;

    private GameObject _lastSelected;
    private Coroutine _delayRoutine;

    public Button InitialButton() => initialButton;

    public void Toggle(bool isOpen) {
        if (_delayRoutine != null) StopCoroutine(_delayRoutine);

        gameObject.SetActive(isOpen);

        if (isOpen) {
            HandleOpen();
        }
        else {
            HandleClose();
        }
    }
    
    public void Open() => Toggle(true);
    public void Close() => Toggle(false);

    private void HandleOpen() {
        SetInitialSelection();
        ToggleHud(false);
        StartPauseDelay();
    }

    private void HandleClose() {
        ToggleHud(true);
        SetPause(false);
    }

    private void SetInitialSelection() {
        if (eventSystem && initialButton) {
            eventSystem.SetSelectedGameObject(initialButton.gameObject);
        }
    }

    private void ToggleHud(bool visible) {
        if (onToggleHudChannel) {
            onToggleHudChannel.RaiseEvent(visible);
        }
    }

    private void SetPause(bool paused) {
        if (onOpenMenuGamePauseChannel) {
            onOpenMenuGamePauseChannel.RaiseEvent(paused);
        }
    }

    private void StartPauseDelay() {
        if (!onOpenMenuGamePauseChannel) return;
        _delayRoutine = StartCoroutine(PauseAfterDelay());
    }

    private IEnumerator PauseAfterDelay() {
        if (pauseDelay > 0f) {
            yield return new WaitForSecondsRealtime(pauseDelay);
        }
        SetPause(true);
        _delayRoutine = null;
    }

    private void Update() {
        if (!eventSystem) return;

        GameObject current = eventSystem.currentSelectedGameObject;
        if (current && current != _lastSelected) {
            _lastSelected = current;
        }
        else if (!current && _lastSelected) {
            eventSystem.SetSelectedGameObject(_lastSelected);
        }
    }
}
