using System;
using UnityEngine;

public interface ICharacterBehaviourStrategy : ICloneable {
    /// <summary>
    /// Returns where the entity should be facing during each action
    /// </summary>
    public Vector2 GetDirectionVector();
    /// <summary>
    /// Returns if the character is currently standing at their target position
    /// </summary>
    public bool GetIsAtTargetPosition();
    /// <summary>
    /// Used to draw gizmos for the AI.
    /// </summary>
    public float GetComfortRadius();
    /// <summary>
    /// Used to draw gizmos for the AI.
    /// </summary>
    /// <returns></returns>
    public float GetAwarenessRadius();
    /// <summary>
    /// Handles the movement behaviour of the AI, put this inside a FixedUpdate block.
    /// </summary>
    public void HandleMovement(Transform transform, Rigidbody2D rb, Transform targetTransform, float movementSpeed, Vector2 pushVelocity);
    /// <summary>
    /// If the behaviour stores data, this functions resets the data to defaults
    /// </summary>
    public void Reset();
}
