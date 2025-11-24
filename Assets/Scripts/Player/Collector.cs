using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Collector : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Min(0)] private float collectRadius = 3f;
    [SerializeField, Min(0)] private float batchCollectInterval = 0.05f;
    
#if UNITY_EDITOR
    [Header("Visual Settings")]
    [SerializeField] private bool drawRadiusGizmo = true;
    [SerializeField] private Color gizmoColor = Color.magenta;
#endif

    private CentralizedFactory _centralizedFactory;

    private Coroutine _collectAllCoroutine;
    
    private IEnumerator CollectAllCollectiblesRoutine(GameObject collectiblePrefab) {
        if (!collectiblePrefab || !_centralizedFactory) {
            yield break;
        }

        List<GameObject> allCollectibleObjects = _centralizedFactory.GetAllActiveObjects(collectiblePrefab);

        if (allCollectibleObjects.Count <= 0) {
            yield break;
        }

        Debug.Log($"{name}: collectibles found: {allCollectibleObjects.Count}!");

        foreach (GameObject collectibleObject in allCollectibleObjects) {
            if (!gameObject.activeInHierarchy) {
                yield break;
            }

            if (collectibleObject && collectibleObject.TryGetComponent<ICollectable>(out ICollectable collectible)) {
                collectible.Collect(gameObject);
            }

            if (batchCollectInterval > 0f) {
                yield return new WaitForSeconds(batchCollectInterval);
            }
            else {
                yield return null;
            }
        }

        _collectAllCoroutine = null;
    }
    
    private void CancelCollectCoroutine() {
        if (_collectAllCoroutine == null) return;
        
        StopCoroutine(_collectAllCoroutine);
        _collectAllCoroutine = null;
    }
    
    private void CollectAllCollectibles(GameObject collectiblePrefab) {
        CancelCollectCoroutine();

        _collectAllCoroutine = StartCoroutine(CollectAllCollectiblesRoutine(collectiblePrefab));
    }
    
    private void HandleTrigger(Collider2D collision) {
        if (!enabled) return;
        if (!collision.TryGetComponent(out ICollectable collectable)) return;
        collectable.Collect(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        HandleTrigger(collision);    
    }

    private void Start() {
        if (!ServiceLocator.TryGetService(out _centralizedFactory)) {
            Debug.LogWarning($"{name}: unable to find centralized factory!");
        }
    }

    private void OnValidate() {
        if (!TryGetComponent(out CircleCollider2D col)) {
            Debug.LogError($"{name}: missing reference \"{nameof(col)}\"");
            return;
        }
        col.isTrigger = true;
        col.radius = collectRadius;
    }

    private void OnEnable() {
        CollectAllCollectiblesCollectible.OnCollectAllCollectibles += CollectAllCollectibles;
    }

    private void OnDisable() {
        CollectAllCollectiblesCollectible.OnCollectAllCollectibles -= CollectAllCollectibles;
        CancelCollectCoroutine();
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (!drawRadiusGizmo) return;
        UnityEditor.Handles.color = gizmoColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, collectRadius);
    }
#endif
}
