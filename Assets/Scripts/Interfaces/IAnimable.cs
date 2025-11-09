using UnityEngine;

public interface IAnimable {
    public Vector2 GetFacingDirection();
    public Vector2 GetMovementDirection();
    public float GetCurrentHealth();
}
