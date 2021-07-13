using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI.Helper;
using AI.Zombie.Helper;


public class ZombieController : MonoBehaviour
{
    #region variables
    [Range(1, 10)] public float stopDistance = 1f;

    private bool _chasingPlayer;
    public bool ChasingPlayer 
    {
        get { return _chasingPlayer; }
    }

    private ZombieAnimController _zAnimController;
    public ZombieAnimController ZombieAnimController
    {
        get { return _zAnimController; }
    }

    public bool Agro
    {
        get { return _zAnimController.Agro; }
    }


    private TargetPriorityLevel _currentTargetPriority;
    private Vector3 _currentTargetPosition;
    private NavMeshPath _currentPath;

    private Coroutine _walkTowardsPositionIE;
    private Coroutine _agroTowardsPositionIE;

    #endregion

    private void Awake()
    {
        #region init
        _currentTargetPosition = Vector3.zero;
        _currentTargetPriority = TargetPriorityLevel.none;

        _currentPath = new NavMeshPath();
        _zAnimController = new ZombieAnimController(GetComponent<Animator>());
        #endregion
    }


    public IEnumerator WalkTowardsPosition(Vector3 position) {
        _zAnimController.Patrolling = true;
        _walkTowardsPositionIE = StartCoroutine( GoTowardsPosition(position) );
        yield return _walkTowardsPositionIE;
        _zAnimController.Patrolling = false;
    }

    

    public IEnumerator AgroZombieBySound(Vector3 soundPosition,TargetPriorityLevel priorityLevel)
    {
        if (_chasingPlayer || !ValidateNewTarget(soundPosition, priorityLevel) )
            yield break;

        if (_currentTargetPosition != Vector3.zero) // if its already chasing a sound
        {
            yield return _zAnimController.Agro == true;
            _currentTargetPosition = soundPosition;
        }
        else // if it wasnt chasing anything
        {
            transform.LookAt(soundPosition);

            _zAnimController.StartAgro();
            yield return new WaitForSeconds(2); // Wait until the startAgro anim. end

            _agroTowardsPositionIE = StartCoroutine( GoTowardsPosition(soundPosition) );
            yield return _agroTowardsPositionIE;

            _zAnimController.StopAgro();

            _currentTargetPosition = Vector3.zero; // finished the path
            yield break;
        }
    }


    private IEnumerator GoTowardsPosition(Vector3 targetPosition)
    {
        _currentTargetPosition = targetPosition;

        float refreshTime = 0.2f;
        while (true)
        {
            NavMesh.CalculatePath(transform.position, _currentTargetPosition, -1, _currentPath);
            AI_DebugPath.DebugPath(_currentPath);

            if (_currentPath.corners.Length  <= 2) // the last 2 corners (the transform pos. and the target pos.)
            {
                if (Vector3.Distance(transform.position, _currentPath.corners[1]) <= stopDistance)
                {
                    yield break;
                }
            }

            transform.LookAt(_currentPath.corners[1]);

            yield return new WaitForSeconds(refreshTime);
        }
    }



    private bool ValidateNewTarget(Vector3 targetPosition, TargetPriorityLevel priorityLevel)
    {
        return ValidateSound(targetPosition) && ValidatePriority(priorityLevel);
    }

    private bool ValidateSound(Vector3 targetPosition)
    {
        if (_currentTargetPosition == Vector3.zero)
        {
            return true;
        }
        else if (_currentTargetPosition == targetPosition)
        {
            return false;
        }
        else
            return (Vector3.Distance(transform.position, targetPosition) < Vector3.Distance(transform.position, _currentTargetPosition));
    }

    private bool ValidatePriority(TargetPriorityLevel priorityLevel) {
        return priorityLevel < _currentTargetPriority;
    }
}