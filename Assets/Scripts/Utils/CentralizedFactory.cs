using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// A centralized factory that also pools objects
/// </summary>
public class CentralizedFactory : MonoBehaviour {
    [Header("Pooling")]
    [SerializeField] private MyObjectPool[] objectPools;

    private List<GameObject> _creations = new List<GameObject>();

    public GameObject Create(GameObject prefabToCreate, Vector3 position, Quaternion rotation, Vector3 scale) {
        if(!prefabToCreate) {
            Debug.LogWarning($"{name}: trying to create null object!");
        }
        foreach (MyObjectPool pool in objectPools) {
           if(pool.GetObjectToPool() == prefabToCreate && pool.GetPooledCount() > 0) {
                return pool.PoolGetRequest(position, rotation, scale);
           }
        }
        
        Debug.LogWarning($"{name}: \"{prefabToCreate.name}\" not pooled, instantiating it instead");
        GameObject obj = Object.Instantiate(prefabToCreate, transform);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = scale;
        _creations.Add(obj);
        return obj;
    }

    public List<GameObject> GetAllActiveObjects(GameObject prefab) {
        List<GameObject> activeObjects = new  List<GameObject>();
        
        foreach (MyObjectPool pool in objectPools) {
            if (pool.GetObjectToPool() != prefab) continue;
            
            GameObject[] pooledObjects = pool.GetPooledObjects();
            foreach (GameObject pooledObject in pooledObjects) {
                if(!pooledObject.activeInHierarchy) continue;
                activeObjects.Add(pooledObject);
            }
        }

        return activeObjects;
    }
    
    private void SoftWipe() {
        foreach (MyObjectPool pool in objectPools) {
            pool.PoolCleanup();
        }
        foreach (GameObject creation in _creations) {
            if (creation && creation.activeInHierarchy) {
                creation.SetActive(false);
            }
        }
    }

    private void Awake() {
        ServiceLocator.SetService(this);
        foreach (MyObjectPool pool in objectPools) {
            pool.Initialize(gameObject);
        }
    }

    private void OnEnable() {
        MyGameManager.OnGameEnd += SoftWipe;
    }

    private void OnDisable() {
        MyGameManager.OnGameEnd -= SoftWipe;
    }
}
