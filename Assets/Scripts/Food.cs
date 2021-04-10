using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float energy = 3f;

    private void OnDestroy()
    {
        Debug.Log("Меня захавали!");
    }
}
