using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event Action<Cell> RemoveCell;
    public static event Action<Cell> DivCell;
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
    public int[] gen = new int[24];
    [Space]
    [SerializeField] private Rigidbody body = null;

    [SerializeField] private float energy;
    private Vector3 movement;
    private bool moveToFood = false;
    private Coroutine changeMovementDirection;
    private HashSet<Cell> cells = new HashSet<Cell>();
    [SerializeField] private Food food = null;

    public bool SymbiosisGene
    {
        get
        {
            return symbiosisGene;
        }

        set
        {
            symbiosisGene = value;
        }
    }

    public void Mutate()
    {
        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance <= mutationChance)
        {
            gen[UnityEngine.Random.Range(0, 24)] = UnityEngine.Random.Range(1, 5);
        }
    }

    private bool CheckGen()
    {
        int count = 0;

        for (int g = 1; g <= 4; g++)
        {
            for (int i = 0; i < 24; i++) 
                if (gen[i] == g) 
                    count++;

            if (count > 8) 
                return false;

            count = 0;
        }

        return true;
    }

    void GenSettings()
    {
        divisionSpeed = startEnergy = speed = size = resistance = 0;

        for (int i = 0; i < 4; i++) 
            divisionSpeed += gen[i];

        for (int i = 4; i < 8; i++) 
            startEnergy += gen[i];

        for (int i = 8; i < 12; i++) 
            speed += gen[i];

        for (int i = 12; i < 16; i++) 
            size += gen[i];

        for (int i = 20; i < 24; i++) 
            resistance += gen[i];

        divisionSpeed = 40f - divisionSpeed;
        startEnergy += 5f;
        size /= 4f;
        speed /= 2f;
    }

    void Start()
    {
        GenSettings();
        SetSize();
        SetEnergy();

        changeMovementDirection = StartCoroutine(ChangeMovementDirection());
    }

    void FixedUpdate()
    {
        CheckCells();
        CheckEnergy();
        if (!moveToFood)
            FindFood();
        Move();
    }

    private void SetSize()
    {
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
        if (food != null)
        {
            movement = -(transform.position - food.transform.position).normalized;
        }

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

        Debug.Log($"� ����! {moveToFood}");
        food = null;
    }

    private IEnumerator ChangeMovementDirection()
    {
        while (true)
        {
            Debug.Log("����������� ��������");
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
        if (energy >= divisionSpeed)
        {
            DivCell?.Invoke(this);
        }

        energy -= Time.fixedDeltaTime;

        if (energy <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void CheckFood()
    {
        if (food != null)
        {
            MoveToFood();
        }
        else
        {
            if (changeMovementDirection == null)
                changeMovementDirection = StartCoroutine(ChangeMovementDirection());
        }
    }

    private void FindFood()
    {
        Collider[] foods = Physics.OverlapSphere(transform.position, viewRadius, 1 << 8);
        float minDistance = Mathf.Infinity;

        foreach (var f in foods)
        {
            if (Vector3.Distance(transform.position, f.transform.position) < minDistance)
            {
                food = f.GetComponent<Food>();
            }
        }

        CheckFood();
    }

    private void MoveToFood()
    {
        moveToFood = true;
        StopCoroutine(changeMovementDirection);
    }
    
    private void SpawnCell(int[] gen)
    {

        GameObject newCell = Instantiate(cellPrefab);
        newCell.GetComponent<Cell>().gen = gen;
        for(int i =0; i < 8; i++)  newCell.GetComponent<Cell>().Mutate();
        newCell.GetComponent<Rigidbody>().velocity = Vector3.zero;
        newCell.GetComponent<Cell>().Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Food"))
        {
            Eat(collision.collider.GetComponent<Food>().energy);
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
        Gizmos.DrawLine(transform.position, transform.position + movement * size);
    }
}
