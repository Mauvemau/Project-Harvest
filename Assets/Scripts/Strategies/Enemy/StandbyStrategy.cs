using UnityEngine;

[System.Serializable]
public class StandbyStrategy : ICharacterBehaviourStrategy {
    public Vector2 GetDirectionVector() => Vector2.zero;

    public bool GetIsAtTargetPosition() => false;
    public float GetComfortRadius() => 0f;
    public float GetAwarenessRadius() => 0f;

    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity) {
        return;
    }

    public void Reset() {}
    
    public object Clone() => MemberwiseClone();
}
