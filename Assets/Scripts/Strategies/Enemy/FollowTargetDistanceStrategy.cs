using UnityEngine;

[System.Serializable]
public class FollowTargetDistanceStrategy : ICharacterBehaviourStrategy {
    [Header("Follow Settings")]
    [SerializeField, Min(0f)] private float stopDistance = 0.5f;

    [Header("Grip Control")]
    [SerializeField] private AnimationCurve gripCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField, Min(0f)] private float gripDuration = 0.5f;

    private readonly FollowTargetStrategy _followStrategy = new FollowTargetStrategy();

    private float _gripTimer = 1f;
    private bool _shouldStop = false;
    private Vector2 _lookDirection = Vector2.zero;

    public Vector2 GetDirectionVector() => _lookDirection;
    public bool GetIsAtTargetPosition() => _shouldStop;
    public float GetComfortRadius() => 0f;
    public float GetAwarenessRadius() => 0f;

    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity) {
        if (!targetTransform || !rb) return;
        
        float distance = Vector2.Distance(transform.position, targetTransform.position);
        _shouldStop = distance <= stopDistance;
        
        float targetGrip = _shouldStop ? 0f : 1f;
        float delta = Time.fixedDeltaTime / Mathf.Max(gripDuration, 0.0001f);
        _gripTimer = Mathf.MoveTowards(_gripTimer, targetGrip, delta);
        
        float speedMultiplier = gripCurve.Evaluate(_gripTimer);
        if (speedMultiplier <= 0.001f) return;
        
        float effectiveSpeed = movementSpeed * speedMultiplier;
        _followStrategy.HandleMovement(transform, rb, targetTransform, effectiveSpeed, pushVelocity);

        _lookDirection = _followStrategy.GetDirectionVector();
    }

    public void Reset() {
        _gripTimer = 1f;
        _shouldStop = false;
        _lookDirection = Vector2.zero;
        _followStrategy.Reset();
    }
    
    public object Clone() => MemberwiseClone();
}
