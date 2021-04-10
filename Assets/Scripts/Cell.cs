using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private bool symbiosisGene;
    [SerializeField] private float divisionSpeed;
    [SerializeField] private float lifespan;
    [SerializeField] private float speed;
    [SerializeField] private float size;
    [SerializeField] private float radiusOfView;
    [SerializeField] private float mutationChance;
    [SerializeField] private float resistance;
    [SerializeField] private float color;
    [Space]
    [SerializeField] new SphereCollider collider = null;

    void Start()
    {
        collider = GetComponent<SphereCollider>();

        SetSize();
        StartCoroutine(Division());
        Destroy(gameObject, lifespan);
    }

    void Update()
    {
        
    }

    private void SetSize()
    {
        collider.radius = size / 2f;
        transform.localScale = new Vector3(size, size, size);
    }

    private void Move()
    {
        
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
}
