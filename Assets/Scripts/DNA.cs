using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    public Material a;
    public Material c;
    public Material t;
    public Material g;
    public Material defaultMaterial;
    public List<MeshRenderer> gensList = new List<MeshRenderer>();

    public void SetGensColor(int[] gens)
    {
        for (int i = 0; i < gens.Length; ++i)
        {
            switch (gens[i])
            {
                case 1:
                    gensList[i].material = a;
                    break;
                case 2:
                    gensList[i].material = c;
                    break;
                case 3:
                    gensList[i].material = t;
                    break;
                case 4:
                    gensList[i].material = g;
                    break;
                default:
                    gensList[i].material = defaultMaterial;
                    break;
            }
        }
    }
}
