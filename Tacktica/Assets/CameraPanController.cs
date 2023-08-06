using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 10.0f;

    PlayerInputDispatcher inputDispatcher;

    float yaw = 0;
    const float minZoom = 10.0f;
    const float maxZoom = -30.0f;

    float curZoom;
    Vector3 curVelocity;


    void Start()
    {
        inputDispatcher = GetComponent<PlayerInputDispatcher>();
        curZoom = Camera.main.transform.localPosition.z;
    
    }

    // Update is called once per frame
    void Update()
    {
        // handle rotation
        HandleRotation();
        HandleMovement();
        HandleZoom();

    }

    private void HandleZoom()
    {
        float zoomDelta = inputDispatcher.cameraZoomInput.y;

        curZoom += (zoomDelta * 10.0f) * Time.deltaTime;
        curZoom = Mathf.Clamp(curZoom, -30.0f, 10.0f);

        var pos = Camera.main.transform.localPosition;
        pos.z = curZoom;

        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, pos, 10.0f * Time.deltaTime);
    }

    private void HandleMovement()
    {
        var moveDir = inputDispatcher.cameraMoveInput;
        Vector3 position = transform.position;
        position += transform.forward * moveDir.y * moveSpeed * Time.deltaTime;
        position += transform.right * moveDir.x * moveSpeed * Time.deltaTime;

        transform.position = Vector3.SmoothDamp(transform.position, position, ref curVelocity, 10.0f * Time.deltaTime);
    }

    private void HandleRotation()
    {
        bool enableRotation = inputDispatcher.isRotationEnabled;
        if (enableRotation)
        {
            var delta = inputDispatcher.delta;
            yaw += rotationSpeed * delta.x * Time.deltaTime;
        }
        Quaternion rotation = Quaternion.Euler(new Vector3(0, yaw, 0));

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10.0f * Time.deltaTime);
        transform.rotation = rotation;
    }

}
