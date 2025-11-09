using UnityEngine;

[System.Serializable]
public class KeepDistanceStrategy : ICharacterBehaviourStrategy {
    // When the target gets past our comfort radius, we begin to flee.
    [SerializeField, Min(0)] private float comfortRadius = 7.0f;
    // If the target is beyond our awareness radius we get closer.
    [SerializeField, Min(0)] private float awarenessRadius = 12.0f;
    // If target is inside the awareness radius and beyond the comfort radius we stop moving.
    [Header("Grip Control")]
    [SerializeField] private AnimationCurve gripCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField, Min(0)] private float gripDuration = 0.5f;

    private ICharacterBehaviourStrategy _followStrategy = new FollowTargetStrategy();
    private ICharacterBehaviourStrategy _fleeStrategy = new FleeFromTargetStrategy();

    private ICharacterBehaviourStrategy _currentStrategy = new StandbyStrategy();

    private float _gripTimer = 0f;
    private Vector2 _lookDirection = Vector2.zero;
    private bool _moving = false;

    public Vector2 GetDirectionVector() => _lookDirection;

    public bool GetIsAtTargetPosition() => !_moving;
    public float GetComfortRadius() => comfortRadius;
    public float GetAwarenessRadius() => awarenessRadius;

    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity) {
        if (!rb || !targetTransform) return;

        Vector2 delta = targetTransform.position - transform.position;
        float distance = delta.magnitude;

        bool shouldFlee = distance < comfortRadius;
        bool shouldFollow = distance > awarenessRadius;

        float targetGrip = (shouldFlee || shouldFollow) ? 1f : 0f;

        float deltaTime = Time.fixedDeltaTime / gripDuration;
        _gripTimer = Mathf.MoveTowards(_gripTimer, targetGrip, deltaTime);

        float speedMultiplier = gripCurve.Evaluate(_gripTimer);
        float effectiveSpeed = movementSpeed * speedMultiplier;

        _moving = false;
        if (shouldFlee) {
            _currentStrategy = _fleeStrategy;
            _moving = true;
        }
        if (shouldFollow) {
            _currentStrategy = _followStrategy;
            _moving = true;
        }

        _currentStrategy.HandleMovement(transform, rb, targetTransform, effectiveSpeed, pushVelocity);
        if (shouldFlee || shouldFollow) {
            _lookDirection = _currentStrategy.GetDirectionVector();
        }
        else {
            _lookDirection = delta.normalized;
        }
    }
    
    public void Reset() {
        _gripTimer = 0f;
        _lookDirection = Vector2.zero;
    }
}
