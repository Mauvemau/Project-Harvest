using UnityEngine;

[System.Serializable]
public class LinearShotStrategy : IBulletStrategy {
    [SerializeField] private bool fixedRotation = false;
    
    private Transform _bulletTransform;
    private Rigidbody2D _rb;
    private Vector2 _aimDirection;
    private float _movementSpeed;

    public void Init(Transform bulletTransform, Rigidbody2D rb, Vector2 aimDirection, float movementSpeed, Transform weaponTransform = null) {
        _bulletTransform = bulletTransform;
        _rb = rb;
        _aimDirection = aimDirection;
        _movementSpeed = movementSpeed;
    }

    public void HandleMovement() {
        if (!_rb) return;
        if (_aimDirection.sqrMagnitude < 0.001f) return;

        Vector2 move = _aimDirection.normalized * (_movementSpeed * Time.fixedDeltaTime);
        Vector2 newPosition = _rb.position + move * Time.fixedDeltaTime;

        _rb.MovePosition(newPosition);

        if (!_bulletTransform) return;
        float angle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
        
        if (fixedRotation) return;
        _bulletTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public object Clone() => MemberwiseClone();
}
