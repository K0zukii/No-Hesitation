using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject fuelPrefab;
    [SerializeField] private GameManager gameManager;
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(9.94f,1.01f,80f), // Right lane
        new Vector3(3.45f,1.01f,80f), // Midle lane
        new Vector3(-3.51f,1.01f,80f) // left lane
    };

    //Control randomness
    private int lastLaneIndex = -1;
    private int consecutiveSpawns = 0;

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

    void Start()
    {

    StartCoroutine(SpawnTrafficRoutine());
    StartCoroutine(SpawnFuelRoutine());
    
    }

    //Spawn traffic car, spawn rate increases with the game speed so the difficulty increases too
    IEnumerator SpawnTrafficRoutine()
    {
        while (true)
        {
            float baseDelay = Random.Range(1.0f, 2.5f);
            float aggressiveMultiplier = gameManager.speedMultiplier *1.5f;
            float currentDelay = baseDelay / aggressiveMultiplier;

            currentDelay = Mathf.Max(currentDelay, 0.4f);

            yield return new WaitForSeconds(currentDelay);

            SpawnObstacle();
        }
    }

    //Spawn fuel at random interval
    IEnumerator SpawnFuelRoutine()
    {
        while (true)
        {
            float randomDelay = Random.Range(5.0f,10.0f);
            yield return new WaitForSeconds(randomDelay);

            SpawnFuel();
        }
    }
   
    void SpawnObstacle()
    {
        //Manage the randomness, if 2 cars spawned in the same place, force the spawn in another lane
        int laneIndex = Random.Range(0, spawnPositions.Length);

        if (laneIndex == lastLaneIndex)
        {
            consecutiveSpawns++;

            if (consecutiveSpawns >= 2)
            {
                int shift = Random.Range(1,spawnPositions.Length);
                laneIndex = (laneIndex + shift) % spawnPositions.Length;

                consecutiveSpawns = 0;
            }
        }
        else
        {
            consecutiveSpawns = 0;  
        }
        
        lastLaneIndex = laneIndex;

        Vector3 spawnPos = spawnPositions[laneIndex];

        float randomOffset = Random.Range(-0.8f, 0.8f);
        spawnPos.x += randomOffset;

        int obstacleIndex = laneIndex;

        GameObject obs = obstaclePools[obstacleIndex].Get();

        obs.transform.position = spawnPos;
        obs.transform.rotation = obstaclePrefabs[obstacleIndex].transform.rotation;

        spawnedObjectsMap[obs] = obstaclePools[obstacleIndex];
    }

    void SpawnFuel()
    {
        int laneIndex = Random.Range(0, spawnPositions.Length);
        Vector3 spawnPos = spawnPositions[laneIndex];

        GameObject fuel = fuelPool.Get();
        
        fuel.transform.position = spawnPos;
        fuel.transform.rotation = fuelPrefab.transform.rotation;

        spawnedObjectsMap[fuel] = fuelPool;
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
}
