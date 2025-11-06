using UnityEngine;

public interface IControllableCamera {
    public Vector3 GetScreenToWorldPoint(Vector2 mousePosition);
    public Camera GetCameraReference();
}
