using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable, IPushable, IFacingDirection {
    [Header("References")] 
    [Tooltip("The entity to target for behaviour")]
    [SerializeField] private GameObject threatTargetReference;
    [Tooltip("A reference to the sprite, used for damage feedback")]
    [SerializeField] private SpriteRenderer characterSpriteReference;
    [Tooltip("A health bar will be updated based on the entity's health values if set")]
    [SerializeField] private HealthBar healthBarReference;
    [Tooltip("Set if you want the enemy to directly control a weapon")] 
    [SerializeField] private Weapon weaponRemoteControl;

    [Header("Behaviour Settings")]
    [SerializeReference, SubclassSelector] private ICharacterBehaviourStrategy currentBehaviour = new StandbyStrategy();

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField, Range(0f, 1f)] private float pushResistanceMultiplier = 1f;
    [SerializeField] private bool alwaysFaceTarget = false;
    
    [Header("Feedback Settings")] 
    [SerializeField] private DamageFeedbackSprite damageFeedbackManager;

    [Header("Drops Settings")] 
    [SerializeField] private DropManager dropManager;
    
    [Header("General Settings")] 
    [Tooltip("Finds a PlayerCharacter and assigns it as threat target reference automatically")]
    [SerializeField] private bool findPlayer = false;
    [Tooltip("The GameObject is disabled if currentHealth reaches 0")]
    [SerializeField] private bool disableOnDeath = false;

#if UNITY_EDITOR
    [Header("Gizmo Settings")]
    [SerializeField] private bool drawAIGizmo = false;
    [SerializeField] private Color awarenessGizmoColor = Color.green;
    [SerializeField] private Color comfortGizmoColor = Color.red;
#endif

    private Rigidbody2D _rb;
    private bool _alive;
    private Vector2 _pushVelocity;

    //

    public Vector2 GetFacingDirection() {
        if (alwaysFaceTarget && threatTargetReference) {
            return (threatTargetReference.transform.position - transform.position).normalized;
        }
        else {
            return currentBehaviour.GetDirectionVector();
        }
    }

    //

    [ContextMenu("Debug - Revive")]
    public void Revive() {
        currentHealth = maxHealth;
        _alive = true;
        _pushVelocity = Vector2.zero;
        UpdateHealthBar();
    }
    
    [ContextMenu("Debug - Kill")]
    public void Kill() {
        if (!_alive) return;
        currentHealth = 0;
        _alive = false;
        
        dropManager.HandleRequestDrops(transform.position);
        
        if (disableOnDeath) {
            gameObject.SetActive(false);
        }
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
    
    public void Heal(float value) {
        SetCurrentHealth(currentHealth + value);
        HandleDamageFeedback(value);
    }
    
    public void TakeDamage(float damage) {
        SetCurrentHealth(currentHealth - damage);
        HandleDamageFeedback(damage);
    }
    
    public void RequestPush(Vector2 direction, float force) {
        if (!_rb || !_alive) return;
        _pushVelocity = direction * force;
    }

    public void RequestMovement(Vector2 direction) {
        Debug.LogWarning($"{name}: trying to request movement on an entity that can't be externally moved!");
    }

    [ContextMenu("Debug - Find Player")]
    private void TryFindThreatTarget() {
        if (!findPlayer || !ServiceLocator.TryGetService(out PlayerCharacter player)) return;
        if (player == null) {
            Debug.LogWarning($"{name}: Unable to find threat target!");
        }
        else {
            threatTargetReference = player.gameObject;
        }
    }
    
    public void SetThreatTarget(GameObject target) {
        threatTargetReference = target;
    }
    
    //

    private void HandleDamageFeedback(float damageReceived) {
        if (!characterSpriteReference) return;
        damageFeedbackManager.PlayDamageFeedback(damageReceived);
    }
    
    private void UpdateHealthBar() {
        if (!healthBarReference) return;
        healthBarReference.SetMaxValue(maxHealth);
        healthBarReference.SetCurrentValue(currentHealth);
        healthBarReference.gameObject.SetActive(currentHealth < maxHealth);
    }

    private void UpdateWeaponControl() {
        if (!weaponRemoteControl) return;
        if (!_alive || currentBehaviour == null || !threatTargetReference) return;

        if (!currentBehaviour.GetIsAtTargetPosition()) return;
        weaponRemoteControl.PerformManualAttack();
    }
    
    private void BaseInit() {
        _alive = false;
        Revive();
    }

    private void FixedUpdate() {
        if (!_alive || currentBehaviour == null || !threatTargetReference) return;
        Vector2 pushVelocity = _pushVelocity;
        pushVelocity *= pushResistanceMultiplier;

        currentBehaviour.HandleMovement(gameObject.transform, _rb, threatTargetReference.transform, movementSpeed, pushVelocity);

        _pushVelocity *= 0.9f;
    }

    private void Update() {
        damageFeedbackManager.Update();
        UpdateWeaponControl();
    }

    private void Awake() {
        if (!TryGetComponent(out _rb)) {
            Debug.LogError($"{name}: missing reference \"{nameof(_rb)}\"");
        }
        BaseInit();
        damageFeedbackManager.Init(characterSpriteReference);
    }
    
    private void OnEnable() {
        Revive();
        TryFindThreatTarget();
    }

    private void OnDisable() {
        damageFeedbackManager.Reset();
    }

    private void OnDrawGizmos() {
#if UNITY_EDITOR
        if (currentBehaviour == null) return;
        if (!drawAIGizmo) return;
        UnityEditor.Handles.color = comfortGizmoColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, currentBehaviour.GetComfortRadius());
        UnityEditor.Handles.color = awarenessGizmoColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, currentBehaviour.GetAwarenessRadius());
#endif
    }
}
