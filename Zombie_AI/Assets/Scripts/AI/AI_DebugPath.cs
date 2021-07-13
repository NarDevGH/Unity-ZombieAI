using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace AI.Helper {
    public class AI_DebugPath : MonoBehaviour
    {
        public static void DebugPath(NavMeshPath path)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.green, .5f, false);
            }
        }
    }
}
