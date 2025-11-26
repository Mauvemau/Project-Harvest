using UnityEngine;

[System.Serializable]
public class FollowTargetSinePatternStrategy : ICharacterBehaviourStrategy {
    [SerializeField, Min(0f)] private float sineAmplitude = 1f;
    [SerializeField, Min(0f)] private float sineFrequency = 2f;
    
    [Header("Stopping Settings")]
    [SerializeField, Min(0f)] private float stopDistance = 0.5f;
    [SerializeField] private bool lockOnTargetPosition = false;

    [Header("Grip Control")]
    [SerializeField] private AnimationCurve gripCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField, Min(0f)] private float gripDuration = 0.5f;

    private Vector3 _targetPosition = Vector3.zero;
    private Vector2 _movementDirection = Vector2.zero;
    private bool _locked = false;
    private bool _shouldStop = false;
    private float _time;
    private float _gripTimer = 1f;

    private void HandleLockingIn(Transform targetTransform) {
        if (_locked) return;
        
        _targetPosition = targetTransform.position;
        if (lockOnTargetPosition) {
            _locked = true;
        }
    }

    private bool IsWithinStopDistance(Transform transform) {
        if (stopDistance <= 0f) return false;
        float distance = Vector2.Distance(transform.position, _targetPosition);
        
        return distance <= stopDistance;
    }
    
    public Vector2 GetDirectionVector() => _movementDirection;

    public bool GetIsAtTargetPosition() => _shouldStop;
    public float GetComfortRadius() => 0f;
    public float GetAwarenessRadius() => 0f;
    
    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity) {
        if (!targetTransform || !rb) return;
        _time += Time.fixedDeltaTime;

        HandleLockingIn(targetTransform);
        if (!_locked) {
            _targetPosition = targetTransform.position;
        }

        _shouldStop = IsWithinStopDistance(transform);

        float targetGrip = _shouldStop ? 0f : 1f;
        float delta = Time.fixedDeltaTime / Mathf.Max(gripDuration, 0.0001f);
        _gripTimer = Mathf.MoveTowards(_gripTimer, targetGrip, delta);

        float speedMultiplier = gripCurve.Evaluate(_gripTimer);
        if (speedMultiplier <= 0.001f) {
            return;
        }
        
        _movementDirection = (_targetPosition - transform.position).normalized;

        Vector2 perpendicular = new Vector2(-_movementDirection.y, _movementDirection.x);
        float sineOffset = Mathf.Sin(_time * sineFrequency) * sineAmplitude;

        Vector2 sineWaveMotion = (_movementDirection * movementSpeed + perpendicular * sineOffset) * (Time.fixedDeltaTime * speedMultiplier);

        Vector2 newPosition = rb.position + sineWaveMotion * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
    
    public void Reset() {
        _targetPosition = Vector3.zero;
        _movementDirection = Vector2.zero;
        _locked = false;
        _shouldStop = false;
        _time = 0f;
        _gripTimer = 1f;
    }
    
    public object Clone() => MemberwiseClone();
}
