using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimator : MonoBehaviour
{
    AgentLocomotion locomotion;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        locomotion = GetComponent<AgentLocomotion>();    
    }

    void Update()
    {
        anim.SetFloat("Velocity", Mathf.Lerp(anim.GetFloat("Velocity"), locomotion.Velocity.magnitude, 10.0f * Time.deltaTime));
    }
}
