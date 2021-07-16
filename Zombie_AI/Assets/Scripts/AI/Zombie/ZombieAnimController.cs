using UnityEngine;

public class ZombieAnimController : MonoBehaviour
{
    private Animator _zAnimator;

    public ZombieAnimController(Animator zAnimator, float zombieVariant=0f)
    {
        this._zAnimator = zAnimator;
        ZombieVariant = zombieVariant;

        Agro = false;
        Patrolling = false;
    }


    public float ZombieVariant
    {
        get { return _zAnimator.GetFloat("ZombieVariant"); }
        set
        {
            _zAnimator.SetFloat("ZombieVariant", value);
        }
    }

    public bool Agro
    {
        get { return _zAnimator.GetBool("Agro"); }
        set 
        { 
            _zAnimator.SetBool("Agro", value); 
        }
    }

    public bool Patrolling
    {
        get { return _zAnimator.GetBool("Patrolling"); }
        set
        {
            _zAnimator.SetBool("Patrolling", value);
        }
    }

    public bool Crawling
    {
        get { return _zAnimator.GetBool("Crawl"); }
        set
        {
            _zAnimator.SetBool("Crawl", value);
        }
    }


    public bool Attack
    {
        get { return _zAnimator.GetBool("Attack"); }
        set 
        { 
            _zAnimator.SetBool("Attack", value); 
        }
    }


    public void StopAgro() 
    {
        _zAnimator.SetTrigger("stopAgro");
        Agro = false;
    }

    public void Jump() 
    {
        _zAnimator.SetTrigger("Jump");
    }
}
