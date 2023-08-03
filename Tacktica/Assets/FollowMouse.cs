using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(screenRay, out RaycastHit hit))
        {
            transform.position = hit.point;
        }
    }
}
