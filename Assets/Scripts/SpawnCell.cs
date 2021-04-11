using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCell : MonoBehaviour
{
    public GameObject cellPrefab;

    private void Start()
    {
        Spawn();

        Cell.DivCell += DivCell;
    }

    public void Spawn()
    {
        GameObject cell = Instantiate(cellPrefab);

        for (int i = 0; i < 32; i++)
        {
            cell.GetComponent<Cell>().Mutate();
        }
    }

    public void DivCell(Cell cell)
    {
        GameObject[] newCells = new GameObject[2];

        for (int i = 0; i < 2; ++i)
        {
            newCells[i] = Instantiate(cellPrefab, cell.transform.position, Quaternion.identity);
            newCells[i].GetComponent<Cell>().gen = cell.gen;

            for (int j = 0; j < 24; ++j)
            {
                newCells[i].GetComponent<Cell>().Mutate();
            }

            newCells[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        Destroy(cell.gameObject);
    }
}
