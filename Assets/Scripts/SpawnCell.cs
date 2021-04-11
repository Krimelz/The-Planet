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

    public void Spawn()
    {
        GameObject cell = Instantiate(cellPrefab);

        for (int i = 0; i < 32; i++)
        {
            cell.GetComponent<Cell>().Mutate();
        }
    }
}
