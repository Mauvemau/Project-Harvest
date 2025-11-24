using System;
using UnityEngine;

public class CollectAllCollectiblesCollectible : Collectible {
    [Header("Collectible Specific Settings")] 
    [SerializeField] private GameObject collectiblePrefab;
    
    public static event Action<GameObject> OnCollectAllCollectibles = delegate {};
    
    protected override void OnCollect() {
        if (!gameObject.activeInHierarchy) return;
        OnCollectAllCollectibles?.Invoke(collectiblePrefab);
    }

    private void Awake() {
        if (!collectiblePrefab) {
            Debug.LogError($"{name}: no collectible prefab set!");
            return;
        }
        if (!collectiblePrefab.TryGetComponent<ICollectable>(out var collectible)) {
            Debug.LogError($"{name}: prefab set is not a collectible!!");
        }
    }
}
