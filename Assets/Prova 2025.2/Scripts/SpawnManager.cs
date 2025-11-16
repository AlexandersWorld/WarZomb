using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Zombie Settings")]
    public GameObject zombiePrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // assign your 3 spawn locations in Inspector

    [Header("Wave Settings")]
    public int zombiesPerWave = 5;
    public float timeBetweenSpawns = 0.5f;
    public float timeBetweenWaves = 10f;

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true) // infinite waves
        {
            currentWave++;

            Debug.Log("Spawning Wave " + currentWave);

            for (int i = 0; i < zombiesPerWave; i++)
            {
                SpawnZombie();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnZombie()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // pick random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
    }
}
