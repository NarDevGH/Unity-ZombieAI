using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Zombie.Helper;

public class test : MonoBehaviour
{
    [Range(1,1000)]public float soundRadius = 100f;
    public TargetPriorityLevel soundType;
    public Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        foreach (Collider collider in Physics.OverlapSphere(transform.position, soundRadius)) {
            if (collider.CompareTag("Zombie"))
            {
                collider.GetComponent<ZombieControllerExp>().AgroZombieBySound(transform.position, soundType);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,soundRadius);
        
    }
}
