using UnityEngine;

/// <summary>
/// Controls a weapon automatically using a scanner
/// </summary>
[RequireComponent(typeof(IWeapon))]
public class AutoWeaponController : WeaponController {
    [Header("References")]
    [SerializeField] private Scanner scannerReference;

    [Header("Accuracy Settings")] 
    [SerializeField] private bool inverted = false;
    
    [Header("Settings")]
    [SerializeField] private float minRangeRadius = 0f;
    [SerializeField] private float pollingRate = 0.1f;

    private float _nextPoll = 0f;

    private void HandleTargetting() {
        GameObject closest = scannerReference.GetClosestOverlap(transform.position);
        if (!closest) {
            AimWeapon(Vector2.zero);
            return;
        }
        if (!closest.TryGetComponent(out IDamageable damageable)) return;

        Vector2 toTarget = (Vector2)closest.transform.position - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        if (minRangeRadius > 0f && distance <= minRangeRadius) {
            AimWeapon(Vector2.zero);
            return;
        }

        Vector2 direction = inverted ? -toTarget.normalized : toTarget.normalized;
        AimWeapon(direction);
    }

    private void Update() {
        if (Time.time < _nextPoll) return;
        _nextPoll = Time.time + pollingRate;

        HandleTargetting();
    }

    protected override void OnAwake() {
        if (!scannerReference) {
            Debug.LogError($"{name}: missing required component {nameof(CircleCollider2D)}");
        }
    }

    private void OnDrawGizmos() {
#if UNITY_EDITOR
        if (minRangeRadius < 0.1f) return;
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, minRangeRadius);
#endif
    }
}
