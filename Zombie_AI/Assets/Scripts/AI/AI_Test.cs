using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.AI;

public class AI_Test : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

    


    private void Update()
    {
        agent.destination = target.position;



        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(transform.position, out navMeshHit, 1f, NavMesh.AllAreas))
        {
            if (navMeshHit.mask == (int)NavMeshHelper.Areas.Area2)
                print(2);

        }


    }

}


