using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentLocomotion : MonoBehaviour
{
    [Readonly, SerializeField] Vector3 destination;
    [Readonly, SerializeField] Vector3 velocity;

    
    // Update is called once per frame
    void Update()
    {
        //if(destination != null)
        //{
        //    Vector3 lastPosition = transform.position;
        //    transform.position = Vector3.MoveTowards(transform.position, destination, 10.0f * Time.deltaTime);
        //    velocity = (transform.position - lastPosition).normalized;
        
        //    if(velocity.magnitude >= 0.1f)
        //    {
        //        Quaternion lookRotation = Quaternion.LookRotation(velocity);
        //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 20.0f * Time.deltaTime);
        //    }
        
        //}
    }

    public Vector3 Velocity { get => velocity; }
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

   

}

