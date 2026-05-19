using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingSystem : MonoBehaviour
{
    public static PoolingSystem Instance;

    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject fuelPrefab;

    //Objects Pooling
    private IObjectPool<GameObject>[] obstaclePools;
    private IObjectPool<GameObject> fuelPool;
    
    private Dictionary<GameObject, IObjectPool<GameObject>> spawnedObjectsMap = new Dictionary<GameObject, IObjectPool<GameObject>>();

    void Awake() 
    {
        Instance = this;

        obstaclePools = new IObjectPool<GameObject>[obstaclePrefabs.Length];

        // Create the fuel and obstacle pool using lamba to simplify
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            int index = i;
            obstaclePools[i] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(obstaclePrefabs[index]),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false, defaultCapacity: 5, maxSize: 15
            );
        }

        fuelPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(fuelPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false, defaultCapacity: 3, maxSize: 10
        );
    }

    //Get an object and see what pool it goes in using the dictionary
    public void ReturnObjectToPool(GameObject obj)
    {
        if ( spawnedObjectsMap.TryGetValue(obj, out IObjectPool<GameObject> correctPool))
        {
            correctPool.Release(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    public void AddToPool(GameObject obj, IObjectPool<GameObject> pool)
    {
        spawnedObjectsMap[obj] = pool;
    }

    public GameObject GetObstacle(int index)
    {
        GameObject obs = obstaclePools[index].Get();
        AddToPool(obs, obstaclePools[index]);
        return obs;
    }

    public GameObject GetFuel()
    {
        GameObject fuel = fuelPool.Get();
        AddToPool(fuel, fuelPool);
        return fuel;
    }

    public Quaternion GetFuelRotation()
    {
        return fuelPrefab.transform.rotation;
    }

    public Quaternion GetObstacleRotation(int index)
    {
        return obstaclePrefabs[index].transform.rotation;
    }
}