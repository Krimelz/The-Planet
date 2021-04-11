using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DNAViewer : MonoBehaviour, IDragHandler
{
    public GameObject dna;

    private float y;
    private Vector2 currentPosition;
    private Vector2 lastPosition;

    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        y -= deltaMousePos().x;
#elif UNITY_ANDROID
        if (Input.touchCount == 1)
            y -= Input.GetTouch(0).deltaPosition.x;
#endif

        dna.transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
    }

    private Vector2 deltaMousePos()
    {
        currentPosition = Input.mousePosition;
        Vector2 deltaPosition = currentPosition - lastPosition;
        lastPosition = currentPosition;

        return deltaPosition;
    }
}
