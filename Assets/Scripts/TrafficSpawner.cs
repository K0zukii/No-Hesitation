using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(9.94f,1.01f,80f), // Right lane
        new Vector3(3.45f,1.01f,80f), // Midle lane
        new Vector3(-3.51f,1.01f,80f) // left lane
    };

    //Other script reference
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PoolingSystem poolScript;

    //Control randomness
    private int lastLaneIndex = -1;
    private int consecutiveSpawns = 0;

    void Start() 
    {
        StartCoroutine(SpawnTrafficRoutine());
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

        GameObject obs = poolScript.GetObstacle(obstacleIndex);

        obs.transform.position = spawnPos;
        obs.transform.rotation = poolScript.GetObstacleRotation(obstacleIndex);
    }
}