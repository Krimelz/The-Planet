using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    public float distanceToObject = -5f;
    [Space]
    public float rotationSpeed = 5f;
    public float shiftSpeed = 1f;
    public float scaleSpeed = 1f;
    [Space]
    public float rotationLerp = 5f;
    public float shiftLerp = 5f;
    public float scaleLerp = 5f;

    private Quaternion rotation;
    private Vector3 shift;
    private Camera mainCamera;
    private float rotationX;
    private float rotationY;
    private float shiftX;
    private float shiftY;
    private float shiftZ;
    private float oldDistance;
    public bool IsCellViewing { get; set; }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && !IsCellViewing)
        {
            DeltaMousePos();
            Shift();
        }

        if (Input.GetMouseButton(2))
        {
            DeltaMousePos();
            Rotate();
        }

        if (Input.mouseScrollDelta != Vector2.zero)
        {
            Scale();
        }

#elif UNITY_ANDROID
        if (Input.touchCount == 0)
        {
            oldDistance = -1f;
        }

        if (Input.touchCount == 1)
        {
            Rotate();
        }
        else if (Input.touchCount == 2)
        {
            Vector2 firstTouchDelta = Input.GetTouch(0).deltaPosition;
            Vector2 secondTouchDelta = Input.GetTouch(1).deltaPosition;

            float dot = Vector2.Dot(firstTouchDelta.normalized, secondTouchDelta.normalized);

            if (dot <= -0.8f)
            {
                Scale();
            }
            else if (dot >= 0.8f && !IsCellViewing)
            {
                Shift();
            }
        }
#endif
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            shift,
            shiftLerp * Time.fixedDeltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotation,
            rotationLerp * Time.fixedDeltaTime);

        mainCamera.transform.localPosition = new Vector3(
            0f,
            0f,
            Mathf.Lerp(mainCamera.transform.localPosition.z, distanceToObject, scaleLerp * Time.fixedDeltaTime));
    }

    private void Rotate()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        rotationX -= deltaPosition.y * rotationSpeed * Time.fixedDeltaTime;
        rotationY += deltaPosition.x * rotationSpeed * Time.fixedDeltaTime;
#elif UNITY_ANDROID
        rotationX -= Input.GetTouch(0).deltaPosition.y * rotationSpeed * Time.fixedDeltaTime;
        rotationY += Input.GetTouch(0).deltaPosition.x * rotationSpeed * Time.fixedDeltaTime;
#endif
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    private void Scale()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        distanceToObject += Input.mouseScrollDelta.y;
#elif UNITY_ANDROID
        Vector2 firstTouchPos = Input.GetTouch(0).position;
        Vector2 secondTouchPos = Input.GetTouch(1).position;

        float newDistance = Vector2.Distance(firstTouchPos, secondTouchPos);

        if (oldDistance > 0f)
        {
            distanceToObject -= (oldDistance - newDistance) * scaleSpeed * Time.deltaTime;
        }

        oldDistance = newDistance;
#endif
        if (distanceToObject >= -0.05f)
        {
            distanceToObject = -0.05f;
        }
    }

    private void Shift()
    {
        if (!IsCellViewing)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            shiftX += deltaPosition.x * shiftSpeed * Time.fixedDeltaTime;
            shiftY += deltaPosition.y * shiftSpeed * Time.fixedDeltaTime;
#elif UNITY_ANDROID
            shiftX += Input.GetTouch(0).deltaPosition.x * shiftSpeed * Time.fixedDeltaTime;
            shiftY += Input.GetTouch(0).deltaPosition.y * shiftSpeed * Time.fixedDeltaTime;
#endif
        }

        shift = rotation * new Vector3(-shiftX, -shiftY, 0f);
    }

    public void Shift(Cell cell)
    {
        shiftX = -cell.transform.position.x;
        shiftY = -cell.transform.position.y;
        shiftZ = -cell.transform.position.z;

        shift = -(new Vector3(shiftX, shiftY, shiftZ));
    }

    private Vector2 currentPosition;
    private Vector2 lastPosition;
    private Vector2 deltaPosition;

    private void DeltaMousePos()
    {
        currentPosition = Input.mousePosition;
        deltaPosition = currentPosition - lastPosition;
        lastPosition = currentPosition;
    }
}
