using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacter : MonoBehaviour, IMovable, IDamageable, IFacingDirection, IAnimable {
    [Header("References")] 
    [SerializeField] private SpriteRenderer characterSpriteReference;
    
    [Header("Health Settings")] 
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    
    [Header("Physics Settings")]
    [SerializeField] private AnimationCurve gripCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float slipDuration = 0.5f;

    [Header("Feedback Settings")] 
    [SerializeField] private DamageFeedbackSprite damageFeedbackManager;

    [Header("Event Invokers")] 
    [SerializeField] private ProgressBarController healthBarController;
    
    public static event Action OnPlayerDeath = delegate {};
    
    private Rigidbody2D _rb;
    private Vector2 _inputDir;
    private Vector2 _currentVelocity;
    private Vector2 _spawnPosition;
    private float _slipTimer;
    private float _currentSpeed;
    private bool _alive = false;

    // IAnimable
    
    public Vector2 GetFacingDirection() {
        return GetMovementDirection();
    }

    public Vector2 GetMovementDirection() {
        return _inputDir.normalized;
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }

    // IDamageable

    private void HandleDamageFeedback(float damageReceived) {
        if (!characterSpriteReference) return;
        damageFeedbackManager.PlayDamageFeedback(damageReceived);
    }
    
    private void UpdateHealthBar() {
        healthBarController.UpdateValues(currentHealth, maxHealth);
    }
    
    public void SetMaxHealth(float value) {
        if (value <= 0) {
            Debug.LogWarning($"{name}: Trying to set max health to a value less or equal to zero.");
        }

        float healthPercent = 0f;
        
        if (maxHealth > 0) {
            healthPercent = currentHealth / maxHealth;
        }

        maxHealth = value;
        currentHealth = healthPercent * maxHealth;
        
        UpdateHealthBar();
        
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0 && _alive) {
            Kill();
        }
    }
    
    public void SetCurrentHealth(float value) {
        if (!_alive) return;
        currentHealth = value;
        
        UpdateHealthBar();
        
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;            
        }
        if (currentHealth <= 0) {
            Kill();
        }
    }
    
    public void TakeDamage(float damage) {
        SetCurrentHealth(currentHealth - damage);
        HandleDamageFeedback(damage);
    }
    public void Heal(float value) {
        SetCurrentHealth(currentHealth + value);
        HandleDamageFeedback(value);
    }

    [ContextMenu("Debug - Kill")]
    public void Kill() {
        if (!_alive) return;
        currentHealth = 0;
        _alive = false;
        UpdateHealthBar();
        OnPlayerDeath.Invoke();
    }

    [ContextMenu("Debug - Revive")]
    public void Revive() {
        currentHealth = maxHealth;
        _alive = true;
        UpdateHealthBar();
        SoftInit();
    }
    
    // IMovable
    
    public void RequestMovement(Vector2 direction) {
        _inputDir = direction.normalized;
        _currentSpeed = moveSpeed;
    }

    // PlayerCharacter

    private void HandlePhysics() {
        if (_inputDir.sqrMagnitude > 0f) {
            _slipTimer = 0f;
            _currentVelocity = _inputDir * _currentSpeed;
        } 
        else {
            _slipTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(_slipTimer / slipDuration);
            float factor = 1f - gripCurve.Evaluate(t);
            _currentVelocity *= factor;
        }

        _rb.linearVelocity = _currentVelocity;
    }
        
    private void FixedUpdate() {
        if (!_alive) return;
        HandlePhysics();
    }

    private void Update() {
        damageFeedbackManager.Update();
    }

    private void SoftInit() {
        _inputDir =  Vector2.zero;
        _currentVelocity =  Vector2.zero; ;
        _slipTimer = 0;
        _currentSpeed = 0;
        transform.position = _spawnPosition;
    }
    
    private void BaseInit() {
        _alive = false;
        SoftInit();
        Revive();
    }
    
    private void Awake() {
        if (!TryGetComponent(out _rb)) {
            Debug.LogError($"{name}: missing reference \"{nameof(_rb)}\"");
        }
        BaseInit();
        damageFeedbackManager.Init(characterSpriteReference);
        ServiceLocator.SetService(this);
    }

    private void OnDisable() {
        damageFeedbackManager.Reset();
    }
}
