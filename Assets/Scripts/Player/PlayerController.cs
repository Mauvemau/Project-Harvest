using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private PlayerCharacter playerCharacterReference;
    
    [Header("Event Listeners")] 
    [SerializeField] private VoidEventChannelSO onRequestReviveCharacter;

    private void HealCharacter(float amount) {
        if (!playerCharacterReference) return;
        playerCharacterReference.Heal(amount);
    }

    private void KillCharacter() {
        if (!playerCharacterReference) return;
        playerCharacterReference.Kill();
    }
    
    private void MoveCharacter(Vector2 direction) {
        if (!playerCharacterReference) return;
        playerCharacterReference?.RequestMovement(direction);
    }

    private void PerformCharacterInteraction() {
        Debug.Log("Interacted!");
    }

    private void Awake() {
        if (!playerCharacterReference) {
            Debug.LogError($"{name}: missing required reference \"{nameof(playerCharacterReference)}\"");
        }
        
        MyGameManager.OnGameEnd += playerCharacterReference.Revive;
    }

    private void OnDestroy() {
        MyGameManager.OnGameEnd -= playerCharacterReference.Revive;
    }

    private void OnEnable() {
        InputManager.OnPlayerMoveInputPerformed += MoveCharacter;
        InputManager.OnPlayerInteractInputPerformed += PerformCharacterInteraction;
        InputManager.OnDebugInstantDeathInputPerformed += KillCharacter;
        HealingCollectible.OnHealthPickup += HealCharacter;
        
        if (onRequestReviveCharacter) {
            onRequestReviveCharacter.OnEventRaised += playerCharacterReference.Revive;
        }
    }

    private void OnDisable() {
        InputManager.OnPlayerMoveInputPerformed -= MoveCharacter;
        InputManager.OnPlayerInteractInputPerformed -= PerformCharacterInteraction;
        InputManager.OnDebugInstantDeathInputPerformed -= KillCharacter;
        HealingCollectible.OnHealthPickup -= HealCharacter;
        
        if (onRequestReviveCharacter) {
            onRequestReviveCharacter.OnEventRaised -= playerCharacterReference.Revive;
        }
    }
}
