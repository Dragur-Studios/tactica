using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 10.0f;

    float yaw = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.Q))
        {
            yaw += rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {

            yaw -= rotationSpeed * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, yaw, 0));

        Vector3 position = transform.position;
        position += transform.forward * moveDir.z * moveSpeed * Time.deltaTime;
        position += transform.right * moveDir.x * moveSpeed * Time.deltaTime;

        transform.position = position;
    }
}
