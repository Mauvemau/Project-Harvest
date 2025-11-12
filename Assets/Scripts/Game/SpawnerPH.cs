using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerPH : MonoBehaviour {
    [Header("Factory Settings")]
    [SerializeField] private List<FactoryWeightPair> factories;

    [Header("Spawning Settings")]
    [SerializeField] private bool shouldSpawn = false;
    [SerializeField] private float initialSpawnDelay = 3f;

    [Header("Camera-Based Spawn Logic Settings")]
    [SerializeField] private Camera mainCameraReference; 
    [SerializeField, Min(0.1f)] private float spawnPositionOffset = 1f; 
    [SerializeField] private bool combineCameraAndRectangle = false;

    [Header("Rectangle Spawn Area Settings")]
    [SerializeField, Min(0.1f)] private float spawnAreaWidth = 10f;
    [SerializeField, Min(0.1f)] private float spawnAreaHeight = 10f;

    [Header("Spawn Rate Settings")]
    [SerializeField, Min(0)] private float currentSpawnRate = 1f;
    
#if UNITY_EDITOR
    [Header("Visual Settings")] 
    [SerializeField] private Color gizmosColor = Color.magenta;
#endif

    private Factory _specificEnemyFactory = new Factory();

    private float _baseSpawnRate = 0f;
    private float _nextSpawn = 0f;

    // Public

    public void SetSpawning(bool spawn) {
        _nextSpawn = Time.time + initialSpawnDelay;
        shouldSpawn = spawn;
    }

    public void Wipe() {
        foreach (FactoryWeightPair factoryWp in factories) {
            Factory factory = factoryWp.Factory;
            factory.SoftWipe();
        }
    }

    // Private

    private void SpawnEnemyBatch(List<GameObject> prefabs) {
        if (prefabs.Count <= 0) return;
        _specificEnemyFactory?.SetFindCentralizedFactory(true);
        
        foreach (GameObject prefab in prefabs) {
            if (!prefab) return;
            Debug.Log($"{name}: Spawning \"{prefab.name}\"!");
            _specificEnemyFactory?.SetPrefabToCreate(prefab);
            
            Vector3 spawnPos = GetSpawnPosition();
            _specificEnemyFactory?.Create(spawnPos, Quaternion.identity, Vector3.one);
        }
    }

    private void ChangeSpawnRate(float newSpawnRate) {
        Debug.Log($"{name}: Changing spawn rate to {newSpawnRate}!");
        SetSpawning(newSpawnRate > 0);
        currentSpawnRate = newSpawnRate;
    }

    /// <summary>
    /// Clamp position inside defined rectangle
    /// </summary>
    private Vector3 ClampToRectangle(Vector3 pos) {
        float halfWidth = spawnAreaWidth / 2f - spawnPositionOffset;
        float halfHeight = spawnAreaHeight / 2f - spawnPositionOffset;

        float clampedX = Mathf.Clamp(pos.x - transform.position.x, -halfWidth, halfWidth);
        float clampedY = Mathf.Clamp(pos.y - transform.position.y, -halfHeight, halfHeight);

        return transform.position + new Vector3(clampedX, clampedY, 0f);
    }
    
    /// <summary>
    /// Rectangle-only mode: spawns inside defined rectangle borders
    /// </summary>
    private Vector3 GetSpawnPositionInsideRectangle() {
        float halfWidth = spawnAreaWidth / 2f - spawnPositionOffset;
        float halfHeight = spawnAreaHeight / 2f - spawnPositionOffset;

        float x = Random.Range(-halfWidth, halfWidth);
        float y = Random.Range(-halfHeight, halfHeight);

        return transform.position + new Vector3(x, y, 0f);
    }

    /// <summary>
    /// Camera-aware mode: spawns at random positions on the rectangle edges of the camera view
    /// </summary>
    private Vector3 GetSpawnPositionFromCamera() {
        if (!mainCameraReference || !mainCameraReference.orthographic) {
            return GetSpawnPositionInsideRectangle();
        }
        
        float camHeight = mainCameraReference.orthographicSize * 2f;
        float camWidth = camHeight * mainCameraReference.aspect;

        Vector3 camCenter = mainCameraReference.transform.position;
        float halfWidth = camWidth / 2f;
        float halfHeight = camHeight / 2f;
        
        int side = Random.Range(0, 4);
        float x = 0f, y = 0f;

        switch (side) {
            case 0: // left
                x = camCenter.x - halfWidth - spawnPositionOffset;
                y = Random.Range(camCenter.y - halfHeight, camCenter.y + halfHeight);
                break;
            case 1: // right
                x = camCenter.x + halfWidth + spawnPositionOffset;
                y = Random.Range(camCenter.y - halfHeight, camCenter.y + halfHeight);
                break;
            case 2: // top
                y = camCenter.y + halfHeight + spawnPositionOffset;
                x = Random.Range(camCenter.x - halfWidth, camCenter.x + halfWidth);
                break;
            case 3: // bottom
                y = camCenter.y - halfHeight - spawnPositionOffset;
                x = Random.Range(camCenter.x - halfWidth, camCenter.x + halfWidth);
                break;
        }

        Vector3 pos = new Vector3(x, y, 0f);

        if (combineCameraAndRectangle) {
            pos = ClampToRectangle(pos);
        }

        return pos;
    }

    private Vector3 GetSpawnPosition() {
        return !mainCameraReference ? GetSpawnPositionInsideRectangle() : GetSpawnPositionFromCamera();
    }

    private Factory GetWeightedRandomFactory() {
        if (factories == null || factories.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var pair in factories) totalWeight += pair.Weight;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var pair in factories) {
            cumulative += pair.Weight;
            if (randomValue <= cumulative) return pair.Factory;
        }

        return factories[^1].Factory;
    }

    private void ResetSpawnRate() {
        currentSpawnRate = _baseSpawnRate;
    }

    private void Update() {
        if (!shouldSpawn) return;
        if (_nextSpawn > Time.time) return;
        _nextSpawn = Time.time + currentSpawnRate;

        Vector3 spawnPos = GetSpawnPosition();

        Factory chosenFactory = GetWeightedRandomFactory();
        chosenFactory?.Create(spawnPos, Quaternion.identity, Vector3.one);
    }

    private void Awake() {
        _baseSpawnRate = currentSpawnRate;
    }

    private void OnEnable() {
        ChangeSpawnRateEvent.OnChangeContinuousSpawnRate += ChangeSpawnRate;
        SpawnEnemyEvent.OnSpawnEnemyBatch += SpawnEnemyBatch;
        MyGameManager.OnGameEnd += ResetSpawnRate;
    }

    private void OnDisable() {
        ChangeSpawnRateEvent.OnChangeContinuousSpawnRate -= ChangeSpawnRate;
        SpawnEnemyEvent.OnSpawnEnemyBatch -= SpawnEnemyBatch;
        MyGameManager.OnGameEnd -= ResetSpawnRate;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = gizmosColor;
        Vector3 size = new Vector3(spawnAreaWidth, spawnAreaHeight, 0f);
        Gizmos.DrawWireCube(transform.position, size);
    }
#endif
}

[System.Serializable]
public class FactoryWeightPair {
    [SerializeField] private Factory factory;
    [SerializeField, Min(0)] private float weight = 1f;

    public Factory Factory => factory;
    public float Weight => weight;
}
