using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public int startFoodAmount;
    public GameObject foodPrefab;
    public Vector3 foodSpawnArea;
    public float foodSpawnRate;

    private void Start()
    {
        for (int i = 0; i < startFoodAmount; i++)
        {
            Vector3 spawnPoint = new Vector3(
                UnityEngine.Random.Range(-foodSpawnArea.x, foodSpawnArea.x),
                UnityEngine.Random.Range(-foodSpawnArea.y, foodSpawnArea.y),
                UnityEngine.Random.Range(-foodSpawnArea.z, foodSpawnArea.z)
            );

            Instantiate(foodPrefab, spawnPoint, Quaternion.identity);
        }

        StartCoroutine(SpawnFood());
    }

    private IEnumerator SpawnFood()
    {
        while (true)
        {
            yield return new WaitForSeconds(foodSpawnRate);

            Vector3 spawnPoint = new Vector3(
                UnityEngine.Random.Range(-foodSpawnArea.x, foodSpawnArea.x),
                UnityEngine.Random.Range(-foodSpawnArea.y, foodSpawnArea.y),
                UnityEngine.Random.Range(-foodSpawnArea.z, foodSpawnArea.z)
            );

            Instantiate(foodPrefab, spawnPoint, Quaternion.identity);    
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, foodSpawnArea * 2f);
    }
}
