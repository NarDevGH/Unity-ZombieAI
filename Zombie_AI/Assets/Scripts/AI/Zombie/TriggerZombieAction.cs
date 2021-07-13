using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Zombie.Helper;

class TriggerZombieAction : MonoBehaviour
{
    [SerializeField] private zombieActions _zombieAction = zombieActions.Jump;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            if (other.GetComponent<ZombieController>() != null)
            {
                if (other.GetComponent<ZombieController>().Agro == true)
                {
                    switch (_zombieAction)
                    {
                        case zombieActions.Jump:
                            other.GetComponent<ZombieController>().ZombieAnimController.Jump();
                            break;
                        case zombieActions.Crawl:
                            other.GetComponent<ZombieController>().ZombieAnimController.Crawling = true;
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            if (other.GetComponent<ZombieController>() != null)
            {
                if (other.GetComponent<ZombieController>().Agro == true)
                {
                    switch (_zombieAction)
                    {
                        case zombieActions.Crawl:
                            other.GetComponent<ZombieController>().ZombieAnimController.Crawling = false;
                            break;
                    }
                }
            }
        }
    }
}
