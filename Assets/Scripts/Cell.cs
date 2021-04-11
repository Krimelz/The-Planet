using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event Action<Cell> RemoveCell;
    public GameObject cellPrefab;   
    [SerializeField] private bool symbiosisGene;
    [SerializeField] private float divisionSpeed;
    [SerializeField] private float startEnergy;
    [SerializeField] private float speed;
    [SerializeField] private float size;
    [SerializeField] private float viewRadius;
    [SerializeField] private float mutationChance = 0.75f;
    [SerializeField] private float resistance;
    [SerializeField] private float color;
    [SerializeField] private int[] gen = new int[24];
    [Space]
    [SerializeField] SphereCollider cellCollider = null;
    [SerializeField] SphereCollider cellTrigger = null;
    [SerializeField] Rigidbody body = null;

    private Vector3 movement;
    [SerializeField] private float energy;
    private bool moveToFood = false;
    private Coroutine changeMovementDirection;
    private HashSet<Cell> cells = new HashSet<Cell>();
    private HashSet<Food> foods = new HashSet<Food>();

    public bool SymbiosisGene
    {
        get
        {
            return symbiosisGene;
        }
    }

    void Mutate()
    {
        if (UnityEngine.Random.Range(0f, 1f) <= mutationChance)
        {
            gen[UnityEngine.Random.Range(0, 24)] = UnityEngine.Random.Range(0, 5);
        }
    }

    bool CheckGen()
    {
        int count = 0;
        for (int g = 1; g <= 4; g++)
        {
            for (int i = 0; i < 24; i++) if (gen[i] == g) count++;
            if (count > 8) return false;
            count = 0;
        }
        return true;
    }

    void GenSettings()
    {
        for (int i = 0; i < 4; i++) divisionSpeed += gen[i];
        for (int i = 4; i < 8; i++) startEnergy += gen[i];
        for (int i = 8; i < 12; i++) speed += gen[i];
        for (int i = 12; i < 16; i++) size += gen[i];
        for (int i = 16; i < 20; i++) viewRadius += gen[i];
        for (int i = 20; i < 24; i++) resistance += gen[i];

        divisionSpeed = 25 - divisionSpeed;
        startEnergy += 4;
        speed /= 2;
        viewRadius = 3 + viewRadius / 4;
    }

    void Start()
    {
        for(int i = 0; i < 24; i++)
        {
            Mutate();
        }
        GenSettings();
        SetSize();
        SetViewRadius();
        SetEnergy();

        StartCoroutine(Division());
        changeMovementDirection = StartCoroutine(ChangeMovementDirection());
    }

    void FixedUpdate()
    {
        CheckCells();
        CheckEnergy();
    }

    private void SetSize()
    {
        cellCollider.radius = size / 2f;
        transform.localScale = new Vector3(size, size, size) / 2f;
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        while (true)
        {
            if (transform.localScale.x >= size)
            {
                transform.localScale = new Vector3(size, size, size);
                yield break;
            }

            transform.localScale += new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
            yield return null;
        }
    }

    private void SetViewRadius()
    {
        cellTrigger.radius = viewRadius;
    }

    private void SetEnergy()
    {
        energy = startEnergy;
    }

    public void AddEnergy(float takedEnergy)
    {
        energy += takedEnergy;
    }

    private void Move()
    {
        body.velocity = movement * speed;
    }

    private void Eat(float foodEnergy)
    {
        if (cells.Count == 0)
        {
            AddEnergy(3f);
            moveToFood = false;
        }
        else
        {
            AddEnergy(3f / cells.Count);

            foreach (var cell in cells)
            {
                cell.AddEnergy(3f / cells.Count);
            }
        }

        Debug.Log($"Я поел! {moveToFood}");
    }

    private IEnumerator ChangeMovementDirection()
    {
        while (true)
        {
            Debug.Log("Направление изменено");
            movement = new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)
            ).normalized;

            Move();
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));
        }
        
    }

    private void CheckCells()
    {
        foreach (var cell in cells)
        {
            Vector3 direction = (transform.position - cell.transform.position).normalized;

            body.AddForce(-direction * 10f);
        }
    }

    private void CheckEnergy()
    {
        energy -= Time.fixedDeltaTime;

        if (energy <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Division()
    {
        while (true)
        {
            yield return new WaitForSeconds(divisionSpeed + UnityEngine.Random.Range(-2f, 2f));
            SpawnCell(gen);
            SpawnCell(gen);
            Destroy(gameObject);
        }
    }
    
    private void SpawnCell(int[] gen)
    {

        GameObject newCell = Instantiate(cellPrefab);
        newCell.GetComponent<Cell>().gen = gen;
        newCell.GetComponent<Cell>().Mutate();
        newCell.GetComponent<Rigidbody>().velocity = Vector3.zero;
        newCell.GetComponent<Cell>().Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Food"))
        {
            Eat(collision.collider.GetComponent<Food>().energy);
            changeMovementDirection = StartCoroutine(ChangeMovementDirection());
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            Cell cell = other.GetComponent<Cell>();

            if (cell.SymbiosisGene)
            {
                cell.RemoveCell += (cell) =>
                {
                    cells.Remove(cell);
                };

                cells.Add(cell);
            }
        }

        if (other.CompareTag("Food") && !moveToFood)
        {
            moveToFood = true;
            Debug.Log($"Вижу еду! {moveToFood}");
            StopCoroutine(changeMovementDirection);
            movement = -(transform.position - other.transform.position).normalized;
            Move();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            Cell cell = other.GetComponent<Cell>();

            if (cells.Contains(cell))
            {
                cells.Remove(cell);
            }
        }
    }

    private void OnDestroy()
    {
        RemoveCell?.Invoke(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, size / 2f);
        Gizmos.DrawLine(transform.position, transform.position + movement * 2f);
    }
}
