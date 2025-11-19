using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable, IPushable, IFacingDirection, IAnimable {
    [Header("References")] 
    [Tooltip("The entity to target for behaviour")]
    [SerializeField] private GameObject threatTargetReference;
    [Tooltip("A reference to the sprite, used for damage feedback")]
    [SerializeField] private SpriteRenderer characterSpriteReference;
    [Tooltip("A health bar will be updated based on the entity's health values if set")]
    [SerializeField] private HealthBar healthBarReference;
    [Tooltip("Set if you want the enemy to directly control a weapon")] 
    [SerializeField] private Weapon weaponReference;

    [Header("Behaviour Settings")]
    [SerializeReference, SubclassSelector] private ICharacterBehaviourStrategy currentBehaviour = new StandbyStrategy();

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField, Range(0f, 1f)] private float pushResistanceMultiplier = 1f;
    [SerializeField] private bool alwaysFaceTarget = false;

    [Header("Weapon Control")] 
    [SerializeField] private bool manuallyAttack = false;
    
    [Header("Feedback Settings")] 
    [SerializeField] private DamageFeedbackSprite damageFeedbackManager;

    [Header("SFX Settings")] 
    [SerializeField] private AK.Wwise.Event spawnAudioEvent;
    [SerializeField] private AK.Wwise.Event hurtAudioEvent;
    [SerializeField] private AK.Wwise.Event deathAudioEvent;

    [Header("Drops Settings")] 
    [SerializeField] private DropManager dropManager;
    
    [Header("General Settings")] 
    [Tooltip("Finds a PlayerCharacter and assigns it as threat target reference automatically")]
    [SerializeField] private bool findPlayer = false;
    [Tooltip("The GameObject is disabled if currentHealth reaches 0")]
    [SerializeField] private bool disableOnDeath = false;
    [SerializeField] private float disableDelay = 5f;

#if UNITY_EDITOR
    [Header("Gizmo Settings")]
    [SerializeField] private bool drawAIGizmo = false;
    [SerializeField] private Color awarenessGizmoColor = Color.green;
    [SerializeField] private Color comfortGizmoColor = Color.red;
#endif

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private bool _alive;
    private bool _loaded;
    private Vector2 _pushVelocity;
    private float _disableTimestamp;

    // IAnimable

    public Vector2 GetFacingDirection() {
        if (alwaysFaceTarget && threatTargetReference) {
            return (threatTargetReference.transform.position - transform.position).normalized;
        }
        return currentBehaviour.GetDirectionVector();
    }

    public Vector2 GetMovementDirection() {
        if(!threatTargetReference) return Vector2.zero;
        if (!currentBehaviour.GetIsAtTargetPosition()) {
            return (threatTargetReference.transform.position - transform.position).normalized;
        }
        return Vector2.zero;
    }
    
    public float GetCurrentHealth() => currentHealth;

    //IDamageable

    [ContextMenu("Debug - Revive")]
    public void Revive() {
        currentHealth = maxHealth;
        _alive = true;
        weaponReference?.gameObject.SetActive(true);
        _pushVelocity = Vector2.zero;
        UpdateHealthBar();
    }
    
    [ContextMenu("Debug - Kill")]
    public void Kill() {
        if (!_alive) return;
        currentHealth = 0;
        _alive = false;
        weaponReference?.gameObject.SetActive(false);
        damageFeedbackManager.SetDead();
        
        if (_collider){
            _collider.enabled = false;
        }
        
        deathAudioEvent?.Post(gameObject);
        dropManager.HandleRequestDrops(transform.position);

        _disableTimestamp = Time.time + disableDelay;
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

        if (_alive && damage > 0) {
            hurtAudioEvent?.Post(gameObject);
        }
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

    private void HandleDisabling() {
        if (!disableOnDeath) return;
        if (_disableTimestamp <= 0) return;
        
        if (disableDelay > 0) {
            if (Time.time < _disableTimestamp) return;
            _disableTimestamp = 0;
        }

        gameObject.SetActive(false);
    }
    
    private void HandleDamageFeedback(float damageReceived) {
        if (!characterSpriteReference) return;
        damageFeedbackManager.PlayDamageFeedback(damageReceived);
    }
    
    private void UpdateHealthBar() {
        if (!healthBarReference) return;
        healthBarReference.SetMaxValue(maxHealth);
        healthBarReference.SetCurrentValue(currentHealth);
        healthBarReference.gameObject.SetActive(currentHealth > 0 && currentHealth < maxHealth);
    }

    private void UpdateWeaponControl() {
        if (!manuallyAttack) return;
        if (!weaponReference) return;
        if (!_alive || currentBehaviour == null || !threatTargetReference) return;

        if (!currentBehaviour.GetIsAtTargetPosition()) return;
        weaponReference.PerformManualAttack();
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
        HandleDisabling();
        damageFeedbackManager.Update();
        UpdateWeaponControl();
    }

    private void Awake() {
        if (!TryGetComponent(out _rb)) {
            Debug.LogError($"{name}: missing reference \"{nameof(_rb)}\"");
        }
        if (!TryGetComponent(out _collider)) {
            Debug.LogWarning($"{name}: missing \"{nameof(Collider2D)}\" and won't be able to receive damage!");
        }
        if (!weaponReference && disableDelay > 0) {
            Debug.LogWarning($"{name}: missing reference \"{nameof(weaponReference)}\", weapon will not be disabled on death!");
        }
        if (!weaponReference && manuallyAttack) {
            Debug.LogWarning($"{name}: missing reference \"{nameof(weaponReference)}\", won't be able to manually attack!");
        }
        BaseInit();
        damageFeedbackManager.Init(characterSpriteReference);
    }
    
    private void OnEnable() {
        if (_collider){
            _collider.enabled = true;
        }
        
        currentBehaviour.Reset();
        
        Revive();
        TryFindThreatTarget();
        
        _disableTimestamp = 0;
        
        if (_loaded) {
            spawnAudioEvent?.Post(gameObject);
        }
        _loaded = true;
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
