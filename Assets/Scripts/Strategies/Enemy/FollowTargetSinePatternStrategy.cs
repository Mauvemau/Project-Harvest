using UnityEngine;

public class FollowTargetSinePatternStrategy : ICharacterBehaviourStrategy {
    [SerializeField] private float sineAmplitude = 1f;
    [SerializeField] private float sineFrequency = 2f;
    
    private Vector2 _movementDirection = Vector2.zero;
    private float _time;

    public Vector2 GetDirectionVector() => _movementDirection;

    public float GetComforRadius() => 0f;
    public float GetAwarenessRadius() => 0f;
    
    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity) {
        if (!targetTransform || !rb) return;
        _time += Time.fixedDeltaTime;
        
        _movementDirection = (targetTransform.position - transform.position).normalized;
        
        Vector2 perpendicular = new Vector2(-_movementDirection.y, _movementDirection.x);
        float sineOffset = Mathf.Sin(_time * sineFrequency) * sineAmplitude;
        Vector2 sineWaveMotion = _movementDirection * (movementSpeed * Time.fixedDeltaTime) + perpendicular * (sineOffset * Time.fixedDeltaTime);
        Vector2 newPosition = rb.position + sineWaveMotion + pushVelocity * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }
}
