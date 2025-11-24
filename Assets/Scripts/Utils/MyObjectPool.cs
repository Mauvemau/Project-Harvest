using UnityEngine;

[System.Serializable]
public class MyObjectPool {
    [SerializeField] private GameObject objectToPool;
    [SerializeField, Min(0)] private int poolSize;
    [Tooltip("The pool expands itself upon reaching it's maximum capacity")]
    [SerializeField] private bool expandable = false;
    
    [SerializeField] private GameObject[] pool;

    public GameObject[] GetPooledObjects() => pool;
    public GameObject GetObjectToPool() => objectToPool;
    public int GetPooledCount() => pool.Length;

    /// <summary>
    /// Expands the pool by a specified amount
    /// </summary>
    public void ExpandPool(int amount) {
        if (amount <= 0) return;

        int newSize = poolSize + amount;
        GameObject[] expandedPool = new GameObject[newSize];
        
        for (int i = 0; i < poolSize; i++) {
            expandedPool[i] = pool[i];
        }
        
        // If there's objects inside the pool, we use the same parent these objects have
        Transform parent = poolSize > 0 ? pool[0].transform.parent : null;
        
        for (int i = poolSize; i < newSize; i++) {
            GameObject newObj = Object.Instantiate(objectToPool, parent);
            newObj.SetActive(false);
            expandedPool[i] = newObj;
        }
        
        pool = expandedPool;
        poolSize = newSize;
    }
    
    /// <summary>
    /// Expands the size of a pool by 1
    /// </summary>
    private void ForceExpand() => ExpandPool(1);
    
    /// <summary>
    /// Sets inactive a specific object in the Pool
    /// </summary>
    public void PoolReturn(int id) {
        pool[id].SetActive(false);
    }

    /// <summary>
    /// Sets inactive the first active object in the Pool
    /// </summary>
    public void PoolReturn() {
        int i = 0;
        while(!pool[i].activeInHierarchy && i < pool.Length - 1) {
            i++;
        }
        pool[i].SetActive(false);
    }

    
    /// <summary>
    /// Sets active the first inactive object in the Pool at the specified transform, and returns it.
    /// </summary>
    public GameObject PoolGetRequest(Vector3 position, Quaternion rotation, Vector3 scale) {
        int i = 0;
        while (pool[i].activeInHierarchy && i < pool.Length - 1) {
            i++;
        }
        if(expandable && i == pool.Length - 1) {
            ForceExpand();
        }

        if (!pool[i].activeInHierarchy) {
            pool[i].transform.position = position;
            pool[i].transform.rotation = rotation;
            pool[i].transform.localScale = scale;
            pool[i].SetActive(true);
            return pool[i];
        }
        
        Debug.LogWarning("[!] Object Pool is at full capacity and unable to expand!");
        return null;
    }
    
    /// <summary>
    /// Sets active the first inactive object in the Pool at the specified transform.
    /// </summary>
    public void PoolRequest(Vector3 position, Quaternion rotation, Vector3 scale) => PoolGetRequest(position, rotation, scale);
    
    /// <summary>
    /// Disables all objects in the pool.
    /// </summary>
    public void PoolCleanup() {
        if (pool == null || pool.Length == 0) return;

        foreach (GameObject obj in pool) {
            if (obj != null && obj.activeSelf) {
                obj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Initializes the pool instantiating the specified amount of objects of the specific GameObject inside a parent.
    /// </summary>
    public void Initialize(GameObject parent) {
        if (!objectToPool) {
            Debug.LogError("[!] Unable to initialize object pool; object to pool not set.");
            return;
        }
        pool = new GameObject[poolSize];

        for(int i = 0; i < poolSize; i++) {
            GameObject obj = parent
                ? Object.Instantiate(objectToPool, parent.transform, false)
                : Object.Instantiate(objectToPool);
            obj.SetActive(false);
            pool[i] = obj;
        }
    }
    
    /// <summary>
    /// Initializes the pool instantiating the specified amount of objects of the specific GameObject as Inactive.
    /// </summary>
    public void Initialize() => Initialize(null);
}
