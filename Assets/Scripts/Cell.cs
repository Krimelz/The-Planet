using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event Action<Cell> RemoveCell; 

    [SerializeField] private bool symbiosisGene;
    [SerializeField] private float divisionSpeed;
    [SerializeField] private float lifespan;
    [SerializeField] private float energy;
    [SerializeField] private float speed;
    [SerializeField] private float size;
    [SerializeField] private float viewRadius;
    [SerializeField] private float mutationChance;
    [SerializeField] private float resistance;
    [SerializeField] private float color;
    [Space]
    [SerializeField] SphereCollider cellCollider = null;
    [SerializeField] SphereCollider cellTrigger = null;
    [SerializeField] Rigidbody body = null;

    private Vector3 movement;
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

    void Start()
    {
        SetSize();
        SetViewRadius();
        SetEnergy();

        StartCoroutine(Division());
        changeMovementDirection = StartCoroutine(ChangeMovementDirection());
        
        Destroy(gameObject, lifespan);
    }

    void FixedUpdate()
    {
        CheckCells();
        CheckEnergy();
    }

    private void SetSize()
    {
        cellCollider.radius = size / 2f;
        transform.localScale = new Vector3(size, size, size);
    }

    private void SetViewRadius()
    {
        cellTrigger.radius = viewRadius;
    }

    private void SetEnergy()
    {
        energy = lifespan / 2f;
    }

    public void InverseMovement()
    {
        movement = -movement;
        Move();
    }

    private void Move()
    {
        body.AddForce(movement * speed, ForceMode.VelocityChange);
    }

    private void Eat()
    {
        energy += 3f;
    }

    private IEnumerator ChangeMovementDirection()
    {
        while (true)
        {
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
            yield return new WaitForSeconds(divisionSpeed);
            SpawnCell();
        }
    }

    private void SpawnCell()
    {
        GameObject newCell = Instantiate(gameObject);
        newCell.GetComponent<Rigidbody>().velocity = Vector3.zero;
        newCell.GetComponent<Cell>().Move();

        body.velocity = Vector3.zero;
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Food"))
        {
            Eat();
            StartCoroutine(ChangeMovementDirection());
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

        if (other.CompareTag("Food"))
        {
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
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.DrawSphere(transform.position, size / 2f);
    }
}
