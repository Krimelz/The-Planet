using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCell : MonoBehaviour
{
    public GameObject cellPrefab;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        GameObject cell = Instantiate(cellPrefab);
    }
}
