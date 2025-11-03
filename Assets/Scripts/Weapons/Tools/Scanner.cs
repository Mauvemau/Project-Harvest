using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Scanner : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float scannerRadius = 5f;
    [SerializeField] private LayerMask scanLayer;
    [SerializeField] private bool followParent = true;

    [SerializeField, ReadOnly] private int amountOverlaps = 0;
    
    private readonly HashSet<Collider2D> _currentOverlaps = new HashSet<Collider2D>();
    private Vector3 _fixedWorldPosition =  Vector3.zero;
    private CircleCollider2D _col;
    
    public GameObject GetClosestOverlap(Vector2 position) {
        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D col in _currentOverlaps) {
            if (!col || !col.gameObject.activeInHierarchy)
                continue;

            float dist = ((Vector2)col.transform.position - position).sqrMagnitude;
            if (dist < closestDist) {
                closestDist = dist;
                closest = col.gameObject;
            }
        }

        return closest;
    }

    public GameObject[] GetAllOverlaps() {
        List<GameObject> result = new List<GameObject>();
        foreach (Collider2D col in _currentOverlaps) {
            if (col && col.gameObject.activeInHierarchy)
                result.Add(col.gameObject);
        }
        return result.ToArray();
    }

    public void SetRadius(float radius) {
        if (!_col) return;
        scannerRadius = radius;
        _col.radius = scannerRadius;
    }

    public void SetPosition(Vector2 position, bool refreshImmediately = false) {
        _currentOverlaps.Clear();
        transform.position = position;

        if (refreshImmediately) {
            InstantRefill();
        }

        _fixedWorldPosition = transform.position;
    }

    public void SetFollowParent(bool shouldFollow) {
        followParent = shouldFollow;
        if (!shouldFollow) {
            _fixedWorldPosition = transform.position;
        }
    }

    private void InstantRefill() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, scannerRadius, scanLayer);
        foreach (Collider2D hit in hits) {
            _currentOverlaps.Add(hit);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision) => _currentOverlaps.Add(collision);
    private void OnTriggerExit2D(Collider2D collision) => _currentOverlaps.Remove(collision);

    private void SetUpCollider() {
        if (!TryGetComponent(out _col)) {
            Debug.LogError($"{name}: missing required component {nameof(CircleCollider2D)}");
            return;
        }
        _col.isTrigger = true;
        _col.radius = scannerRadius;

        int inverted = ~0 & ~scanLayer;
        _col.includeLayers = 0;
        _col.excludeLayers = inverted;
    }
    
    private void Awake() {
        SetUpCollider();
        _fixedWorldPosition = transform.position;
    }

    private void LateUpdate() {
        amountOverlaps = _currentOverlaps.Count;
        if (!followParent) {
            transform.position = _fixedWorldPosition;
        }
    }

    private void OnValidate() {
        SetUpCollider();
    }
}

