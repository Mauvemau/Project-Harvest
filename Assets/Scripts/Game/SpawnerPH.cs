using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerPH : MonoBehaviour {
    [Header("Current Multipliers")] 
    [SerializeField] private float currentSpawnDensity;
    
    [Header("Factory Settings")]
    [SerializeField] private List<FactoryWeightPair> factories;

    [Header("Spawning Settings")]
    [SerializeField] private bool shouldSpawn = false;
    [SerializeField] private float initialSpawnDelay = 3f;

    [Header("Camera-Based Spawn Logic Settings")]
    [SerializeField] private Camera mainCameraReference; 
    [SerializeField, Min(0.1f)] private float spawnPositionOffset = 1f; 
    
    [Header("Random Spawn Logic Settings")]
    [SerializeField, Min(0.1f)] private float maxSpawnDistance = 5f;
    [SerializeField, Min(0)] private float currentSpawnRate = 1f;

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
        if (!shouldSpawn) return;
        if (prefabs.Count <= 0) return;
        _specificEnemyFactory?.SetFindCentralizedFactory(true);
        
        foreach (GameObject prefab in prefabs) {
            if (!prefab) return;
            Debug.Log($"{name}: Spawning \"{prefab.name}\"!");
            _specificEnemyFactory?.SetPrefabToCreate(prefab);
            
            Vector3 spawnPos = GetSpawnPositionFromCamera();

            _specificEnemyFactory?.Create(spawnPos, Quaternion.identity, Vector3.one);
        }
    }

    private void ChangeSpawnRate(float newSpawnRate) {
        Debug.Log($"{name}: Changing spawn rate to {newSpawnRate}!");
        SetSpawning(newSpawnRate > 0);
        currentSpawnRate = newSpawnRate;
    }
    
    /// <summary>
    /// Spawns within a square range around the gameObject position
    /// </summary>
    private Vector3 GetRandomSpawnPosition() {
        float x = Random.Range(-maxSpawnDistance, maxSpawnDistance);
        float y = Random.Range(-maxSpawnDistance, maxSpawnDistance);
        return transform.position + new Vector3(x, y, 0f);
    }

    /// <summary>
    /// Camera-aware mode: spawns at random positions on the rectangle edges of the camera view
    /// </summary>
    private Vector3 GetSpawnPositionFromCamera() {
        if (!mainCameraReference || !mainCameraReference.orthographic) {
            return GetRandomSpawnPosition();
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

        return new Vector3(x, y, 0f);
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

        Vector3 spawnPos = GetSpawnPositionFromCamera();

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
}

[System.Serializable]
public class FactoryWeightPair {
    [SerializeField] private Factory factory;
    [SerializeField, Min(0)] private float weight = 1f;

    public Factory Factory => factory;
    public float Weight => weight;
}