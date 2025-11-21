using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Collector : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Min(0)] private float collectRadius = 3f;
    
#if UNITY_EDITOR
    [Header("Visual Settings")]
    [SerializeField] private bool drawRadiusGizmo = true;
    [SerializeField] private Color gizmoColor = Color.magenta;
#endif

    private void HandleTrigger(Collider2D collision) {
        if (!enabled) return;
        if (!collision.TryGetComponent(out ICollectable collectable)) return;
        collectable.Collect(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        HandleTrigger(collision);    
    }

    private void OnDrawGizmos() {
#if UNITY_EDITOR
        if (!drawRadiusGizmo) return;
        UnityEditor.Handles.color = gizmoColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, collectRadius);
#endif
    }

    private void OnValidate() {
        if (!TryGetComponent(out CircleCollider2D col)) {
            Debug.LogError($"{name}: missing reference \"{nameof(col)}\"");
            return;
        }
        col.isTrigger = true;
        col.radius = collectRadius;
    }
}
