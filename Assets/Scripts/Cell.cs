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
    [SerializeField] private float speed;
    [SerializeField] private float size;
    [SerializeField] private float viewRadius;
    [SerializeField] private float mutationChance;
    [SerializeField] private float resistance;
    [SerializeField] private float color;
    [Space]
    [SerializeField] new SphereCollider collider = null;
    [SerializeField] SphereCollider trigger = null;
    [SerializeField] Rigidbody body = null;

    private Vector3 movement;
    private HashSet<Cell> cells = new HashSet<Cell>();

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

        StartCoroutine(Division());
        StartCoroutine(ChangeMovementDirection());
        
        Destroy(gameObject, lifespan);
    }

    void FixedUpdate()
    {
        Move();
        CheckCells();
    }

    private void SetSize()
    {
        collider.radius = size / 2f;
        transform.localScale = new Vector3(size, size, size);
    }

    private void SetViewRadius()
    {
        trigger.radius = viewRadius;
    }

    private void Move()
    {
        body.AddForce(movement * speed);
        //body.velocity = movement * speed;
    }

    private IEnumerator ChangeMovementDirection()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));

        movement = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        ).normalized;
    }

    private void CheckCells()
    {
        foreach (var cell in cells)
        {
            Vector3 direction = (transform.position - cell.transform.position).normalized;

            body.AddForce(-direction * 20f);
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
                    //Debug.Log($"{cell.GetInstanceID()}, {this.GetInstanceID()}");
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
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.DrawSphere(transform.position, size / 2f);
    }
}
