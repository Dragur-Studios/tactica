using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    AgentLocomotion locomotion;

    private void Start()
    {
        locomotion = GetComponent<AgentLocomotion>();
    }



    public void SetDestination(Vector3 destination)
    {
        locomotion.SetDestination(destination);
    }

}
