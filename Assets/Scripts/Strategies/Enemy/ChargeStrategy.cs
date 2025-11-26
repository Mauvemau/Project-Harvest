using UnityEngine;

[System.Serializable]
public class ChargeStrategy : ICharacterBehaviourStrategy {
    [Header("References")] 
    [Tooltip("These Objects will be enabled when the standby timer is about to run out. (Used to visually warn the player)")]
    [SerializeField] private GameObject[] enableOnStandbyWarning;
    
    [Header("Standby & Charge Durations")]
    [SerializeField] private float standbyMinDuration = 1f;
    [SerializeField] private float standbyMaxDuration = 2f;
    [SerializeField] private float chargeMinDuration = 3f;
    [SerializeField] private float chargeMaxDuration = 5f;

    [Header("Daze")]
    [SerializeField] private float dazeDuration = 6f;

    [Header("Obstacle Detection")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleDetectionLength = 1f;

    [Header("Charge Speed Curves")]
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve decelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Steering")]
    [SerializeField, Min(0f)] private float steeringWeight = 1f;

    private float _standbyTimer = 0f;
    private float _chargeTimer = 0f;

    private float _currentStandbyDuration = 1f;
    private float _currentChargeDuration = 1f;

    private bool _isCharging = false;

    private Vector2 _movementDirection = Vector2.zero;

    public Vector2 GetDirectionVector() => _movementDirection;
    public bool GetIsAtTargetPosition() => !_isCharging;
    public float GetComfortRadius() => 0f;
    public float GetAwarenessRadius() => 0f;

    private void HandleVisualFeedback() {
        if (enableOnStandbyWarning.Length <= 0) return;
        
        bool shouldEnable = !_isCharging && _standbyTimer > _currentStandbyDuration * .75;
        foreach (GameObject gameObject in enableOnStandbyWarning) {
            gameObject.SetActive(shouldEnable);
        }
    }
    
    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity) {
        if (!rb || !targetTransform) return;

        if (!_isCharging) {
            HandleStandby();
        }
        else {
            HandleCharge(transform, rb, targetTransform, movementSpeed);
        }
        
        HandleVisualFeedback();
    }
    
    private void HandleStandby() {
        _standbyTimer += Time.fixedDeltaTime;
        
        if (_standbyTimer >= _currentStandbyDuration) {
            StartCharge();
        }
    }

    private void HandleCharge(Transform transform, Rigidbody2D rb, Transform targetTransform, float baseSpeed) {
        _chargeTimer += Time.fixedDeltaTime;

        float normalizedTime = Mathf.Clamp01(_chargeTimer / _currentChargeDuration);

        float acceleration = accelerationCurve.Evaluate(normalizedTime);
        float deceleration = decelerationCurve.Evaluate(1f - normalizedTime);

        float speedMultiplier = acceleration * deceleration;
        float finalSpeed = baseSpeed * speedMultiplier;

        Vector2 desiredDirection = (targetTransform.position - transform.position).normalized;

        _movementDirection = Vector2.Lerp(_movementDirection, desiredDirection, steeringWeight * Time.fixedDeltaTime).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _movementDirection, obstacleDetectionLength, obstacleLayer);
        if (hit) {
            HandleCrash(hit);
            return;
        }

        Vector2 movement = _movementDirection * (finalSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + movement);

        if (_chargeTimer >= _currentChargeDuration) {
            StopCharge();
        }
    }

    private void StartCharge() {
        _currentStandbyDuration = 0;
        _isCharging = true;

        _chargeTimer = 0f;
        _currentChargeDuration = Random.Range(chargeMinDuration, chargeMaxDuration);

        _movementDirection = Vector2.zero;
    }

    private void StopCharge() {
        _isCharging = false;

        _standbyTimer = 0f;
        if (_currentChargeDuration <= 0) {
            _currentStandbyDuration = Random.Range(standbyMinDuration, standbyMaxDuration);
        }

        _movementDirection = Vector2.zero;
    }

    private void HandleCrash(RaycastHit2D hit) {
        _movementDirection = Vector2.Reflect(_movementDirection, hit.normal);

        float remainingChargeTime = _currentChargeDuration - _chargeTimer;
        _chargeTimer += remainingChargeTime * .5f;

        _standbyTimer = 0f;
        _currentStandbyDuration = dazeDuration;
    }

    public void Reset() {
        foreach (GameObject gObject in enableOnStandbyWarning) {
            gObject.SetActive(false);
        }
        
        _isCharging = false;

        _standbyTimer = 0f;
        _chargeTimer = 0f;

        _currentStandbyDuration = Random.Range(standbyMinDuration, standbyMaxDuration);
        _currentChargeDuration = Random.Range(chargeMinDuration, chargeMaxDuration);

        _movementDirection = Vector2.zero;
    }

    public object Clone() => MemberwiseClone();
}
