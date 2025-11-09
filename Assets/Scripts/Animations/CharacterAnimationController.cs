using System;
using UnityEngine;

[RequireComponent(typeof(IAnimable))]
public class CharacterAnimationController : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private SpriteRenderer rendererReference;
    [SerializeField] private Animator animatorReference;

    private IAnimable _animableCharacter;
    private float _lastHealthValue;
    
    private static readonly int Moving = Animator.StringToHash("moving");
    private static readonly int Dead = Animator.StringToHash("dead");
    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int Spawn = Animator.StringToHash("spawn");
    
    private bool IsGamePaused() {
        return Time.timeScale <= 0.0f;
    }
    
    private void UpdateAnimator() {
        if (_animableCharacter == null) return;
        if (IsGamePaused()) return;
        float currentHealth = _animableCharacter.GetCurrentHealth();
        
        animatorReference?.SetBool(Moving, _animableCharacter.GetMovementDirection().magnitude > 0.0f);
        
        if(currentHealth < _lastHealthValue) {
            if (currentHealth > 0) {
                animatorReference?.SetTrigger(Hurt);
            }
            else {
                animatorReference?.SetBool(Dead, true);
            }
        }
        if(currentHealth > _lastHealthValue) {
            animatorReference?.SetBool(Dead, false);
        }
        _lastHealthValue = currentHealth;
    }
    
    private void UpdateMirroring() {
        if (_animableCharacter == null) return;
        if (!rendererReference) return;
        if (IsGamePaused()) return;
        
        if (_animableCharacter.GetFacingDirection().x > 0) {
            rendererReference.flipX = false;
        }
        else if (_animableCharacter.GetFacingDirection().x < 0) {
            rendererReference.flipX = true;
        }
    }
    
    private void Awake() {
        if (!TryGetComponent(out _animableCharacter)) {
            Debug.LogError($"{name}: missing reference \"{nameof(IAnimable)}\"");
        }
    }

    public void Update() {
        UpdateMirroring();
        UpdateAnimator();
    }

    public void OnEnable() {
        animatorReference?.SetTrigger(Spawn);
    }
}
