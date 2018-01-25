using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //Contains NavMeshClasses

public class unitMovement : MonoBehaviour
{

    Animator anim;
    public bool moving;
    public NavMeshAgent agent;
    public Transform pointer;

    // Update is called once per frame
    void Start() {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        //Activates the NavMeshAgent Movement
        anim.SetBool("Moving", moving);
        if (moving)
        {
            
            agent.SetDestination(pointer.position);
            agent.isStopped = false;
            //agent.Resume();
        }
    }
}
