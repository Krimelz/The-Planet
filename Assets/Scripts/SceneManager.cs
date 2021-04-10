using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject foodPrefab;
    public Vector3 foodSpawnArea;
    public float foodSpawnRate;

    private void Start()
    {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Cell"))
        {
            Debug.Log("Мама, я в Дубаи");
            collision.collider.GetComponent<Cell>().InverseMovement();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, foodSpawnArea * 2f);
    }
}
