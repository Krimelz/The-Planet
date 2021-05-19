using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public enum SpawnType
    {
        Box,
        Sphere
    }

    public Viewer viewer;
    public GameObject cellInfo;
    public DNA dna;
    public Text startEnergy;
    public Text energyForDivision;
    public Text speed;
    public Text size;
    public Text currentEnergy;
    public SpawnType spawnType;
    public int startFoodAmount;
    public GameObject foodPrefab;
    public Vector3 foodSpawnArea;
    public float foodSpawnRadius;
    public float foodSpawnRate;

    private Cell selectedCell = null;

    private void Start()
    {
        for (int i = 0; i < startFoodAmount; i++)
        {
            Vector3 spawnPoint;

            if (spawnType == SpawnType.Box)
            {
                spawnPoint = new Vector3(
                    UnityEngine.Random.Range(-foodSpawnArea.x, foodSpawnArea.x),
                    UnityEngine.Random.Range(-foodSpawnArea.y, foodSpawnArea.y),
                    UnityEngine.Random.Range(-foodSpawnArea.z, foodSpawnArea.z)
                );
            }
            else
            {
                spawnPoint = new Vector3(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                ).normalized * foodSpawnRadius;
            }

            Instantiate(foodPrefab, spawnPoint, Quaternion.identity);
        }

        StartCoroutine(SpawnFood());

        Cell.Close += CellClose;
    }

    private void CellClose(Cell cell)
    {
        if (cell == selectedCell)
        {
            HideInfo();
        }
    }

    private IEnumerator SpawnFood()
    {
        while (true)
        {
            yield return new WaitForSeconds(foodSpawnRate);

            Vector3 spawnPoint;

            spawnPoint = new Vector3(
                UnityEngine.Random.Range(-foodSpawnArea.x, foodSpawnArea.x),
                UnityEngine.Random.Range(-foodSpawnArea.y, foodSpawnArea.y),
                UnityEngine.Random.Range(-foodSpawnArea.z, foodSpawnArea.z)
            );


            GameObject food = Instantiate(foodPrefab, spawnPoint, Quaternion.identity);
            float foodSize = UnityEngine.Random.Range(0.05f, 0.15f);
            food.GetComponent<Food>().energy = foodSize * 40f;
            food.transform.localScale = new Vector3(foodSize, foodSize, foodSize);
        }
    }

    private void Update()
    {
        if (viewer.IsCellViewing)
        {
            viewer.Shift(selectedCell);
            currentEnergy.text = selectedCell.CurrentEnergy.ToString();
        }

#if UNITY_STANALONE || UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedCell == null)
                selectedCell = GetCellOnScreen(Input.mousePosition);

            if (selectedCell != null)
            {
                ShowInfo();
            }
        }
#elif UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            if (selectedCell == null)
                selectedCell = GetCellOnScreen(Input.GetTouch(0).position);

            if (selectedCell != null)
            {
                ShowInfo();
            }
        }
#endif
    }

    private Cell GetCellOnScreen(Vector2 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 15f, layerMask: 1 << 9);

        if (hit.collider != null)
        {
            return hit.collider.GetComponent<Cell>();
        }
        else
        {
            return null;
        }
    }

    private void ShowInfo()
    {
        cellInfo.SetActive(true);

        dna.SetGensColor(selectedCell.gen);
        startEnergy.text = selectedCell.StartEnergy.ToString();
        energyForDivision.text = selectedCell.DivisionSpeed.ToString();
        speed.text = selectedCell.Speed.ToString();
        size.text = selectedCell.Size.ToString();
        currentEnergy.text = selectedCell.CurrentEnergy.ToString();
    }

    public void HideInfo()
    {
        selectedCell = null;
        cellInfo.SetActive(false);
        viewer.IsCellViewing = false;
    }

    public void ToCell()
    {
        if (selectedCell != null)
        {
            viewer.IsCellViewing = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnType == SpawnType.Box)
            Gizmos.DrawWireCube(transform.position, foodSpawnArea * 2f);
        else
            Gizmos.DrawWireSphere(transform.position, foodSpawnRadius);
    }
}
