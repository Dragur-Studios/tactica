using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentTests : MonoBehaviour
{
    [SerializeField] Agent testAgent;
    [SerializeField] Transform destinationTransform;

    void Start()
    {
                
    }

    void Update()
    {
       
        testAgent.SetDestination(destinationTransform.position);
                
    }
}
