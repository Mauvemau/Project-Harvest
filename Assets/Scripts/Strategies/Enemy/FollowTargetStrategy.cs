using UnityEngine;

[System.Serializable]
public class FollowTargetStrategy : ICharacterBehaviourStrategy {
    private Vector2 _movementDirection = Vector2.zero;
    
    public Vector2 GetDirectionVector() => _movementDirection;

    public bool GetIsAtTargetPosition() => false;
    public float GetComfortRadius() => 0f;
    public float GetAwarenessRadius() => 0f;

    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity) {
        if (!targetTransform || !rb) return;
        _movementDirection = (targetTransform.transform.position - transform.position).normalized;

        Vector2 move = _movementDirection * (movementSpeed * Time.fixedDeltaTime);
        Vector2 newPosition = rb.position + move + pushVelocity * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }
    
    public void Reset() {
        _movementDirection = Vector2.zero;
    }
}
