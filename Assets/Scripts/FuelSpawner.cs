using System.Collections;
using UnityEngine;

public class FuelSpawner : MonoBehaviour
{
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(9.94f,1.01f,80f), // Right lane
        new Vector3(3.45f,1.01f,80f), // Midle lane
        new Vector3(-3.51f,1.01f,80f) // left lane
    };

    [SerializeField] private PoolingSystem poolScript; 
    void Start() 
    {
        StartCoroutine(SpawnFuelRoutine());
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

    void SpawnFuel()
    {
        int laneIndex = Random.Range(0, spawnPositions.Length);
        Vector3 spawnPos = spawnPositions[laneIndex];

        GameObject fuel = poolScript.GetFuel();
        
        fuel.transform.position = spawnPos;
        fuel.transform.rotation = poolScript.GetFuelRotation();
    }
}