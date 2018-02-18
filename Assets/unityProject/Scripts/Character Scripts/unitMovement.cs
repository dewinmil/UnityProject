using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //Contains NavMeshClasses

public class unitMovement : MonoBehaviour
{
    public bool moving;
    public NavMeshAgent agent;
    public Transform pointer;
    public int tileX;
    public int tileY;

    // Update is called once per frame
    void Update()
    {
        //Activates the NavMeshAgent Movement
        if (moving)
        {
            agent.SetDestination(pointer.position);
            agent.isStopped = false;
            //agent.Resume();
        }
    }
}
