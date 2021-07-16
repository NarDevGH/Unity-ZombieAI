using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Helper;
using UnityEngine.AI;

public class TestPath : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

    void Update()
    {
        agent.destination = target.position;
        AI_Helper.DebugPath(agent.path);
    }

}
