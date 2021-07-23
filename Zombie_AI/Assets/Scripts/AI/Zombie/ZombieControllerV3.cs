using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI.Helper;
using AI.Zombie.Helper;
using System;

public class ZombieControllerV3 : MonoBehaviour
{
    #region variables
    [SerializeField]    [Min(1)] private float DetectPlayersAroundR = 1f;
    [SerializeField]    [Min(1)] private float FovRange = 1f;
    [SerializeField]    [Range(0,360)] private float FovViewAngle;
    [SerializeField]    [Range(0.5f, 2)] private float FovOffset = 0.5f;
    [SerializeField]    [Range(1, 10)] private float stopDistance = 1f;
    [SerializeField]    private LayerMask _obstructViewLayers;

    private float refreshTime = 0.2f;

    private bool _chasingPlayer;
    private bool _lostTrackOfPlayer;
    private bool _goingTowardsRandomSound;

    private ZombieAnimController _zAnimController;
    public ZombieAnimController ZombieAnimController { get { return _zAnimController; } }

    public bool Agro
    {
        get { return _zAnimController.Agro; }
    }

    private TargetPriorityLevel _currentTargetPriority;
    private Vector3 _currentTargetPosition;

    private NavMeshPath _currentPath;

    private List<GameObject> _players;

    #endregion

    private void Awake()
    {

        _currentPath = new NavMeshPath();
        _zAnimController = new ZombieAnimController(GetComponent<Animator>());
        _players = new List<GameObject>();

        #region Init

        _chasingPlayer = false;
        _lostTrackOfPlayer = false;
        _goingTowardsRandomSound = false;

        _currentTargetPosition = Vector3.zero;
        _currentTargetPriority = TargetPriorityLevel.none;

        #endregion
    }

    private void Start()
    {
        StartCoroutine(ZombieLogic());
    }

    private IEnumerator ZombieLogic() 
    {
        while (true) 
        {
            yield return new WaitForSeconds(refreshTime);

            _players.Clear();
            GetPlayersNextToTheZombie(_players);
            GetPlayersInsideFOV(_players);

            if (_players.Count >= 1)
            {
                _currentTargetPosition = ClosestPlayerPosition(_players);

                if (_chasingPlayer == false)
                {
                    _chasingPlayer = true;
                    _currentTargetPriority = TargetPriorityLevel.player;

                    StartChasingPlayer();
                }
                else 
                {
                    if (_lostTrackOfPlayer)
                    {
                        ResumeChasingPlayer();
                    }
                }
            }
            else
            {
                if (_chasingPlayer == true && _lostTrackOfPlayer == false) 
                {
                    _lostTrackOfPlayer = true;
                    StartCoroutine( "LostTrackOfPlayer" );
                }
            }

        }
        
    
    }


    #region Public Methods

    public void AgroZombieBySound(Vector3 soundPosition, TargetPriorityLevel priorityLevel)
    {
        if (_chasingPlayer || !ValidateNewTarget(soundPosition, priorityLevel))
        {
            return;
        }

        _currentTargetPosition = soundPosition;

        StartCoroutine("AgroZombie");

    }

    #endregion


    private void StartChasingPlayer()
    {

        if (_goingTowardsRandomSound)  // if the zombie was already going towards somewhere
        {
            _goingTowardsRandomSound = false;
            StopCoroutine("GoTowardsTargetPosition");

            StartCoroutine("ChasePlayer");
        }
        else
        {
            StartCoroutine("AgroZombie");
        }
    }

    private void ResumeChasingPlayer()
    {
        _lostTrackOfPlayer = false;

        StopCoroutine( "LostTrackOfPlayer" );

        StartCoroutine( "ChasePlayer" );
    }

    private IEnumerator LostTrackOfPlayer()
    {
        StopCoroutine("ChasePlayer");

        yield return StartCoroutine("GoTowardsTargetPosition"); // go towards last seen player position and wait until reach it

        _chasingPlayer = false;
        _lostTrackOfPlayer = false;

        _zAnimController.Agro = false;
    }


    private IEnumerator AgroZombie() 
    {
        transform.LookAt(_currentTargetPosition);

        _zAnimController.Agro = true;
        yield return new WaitForSeconds(2); // Wait until the startAgro anim. end

        if (_chasingPlayer) 
        {
            StartCoroutine( "ChasePlayer" );
        }
        else 
        {
            StartCoroutine( "GoTowardsTargetPosition" );
        }

    }

    private IEnumerator ChasePlayer() 
    {
        while (true)
        {
            NavMesh.CalculatePath(transform.position, _currentTargetPosition, -1, _currentPath);

            if (_currentPath.corners.Length > 0)
            {
                AI_Helper.DebugPath(_currentPath, Color.red);

                if (_currentPath.corners.Length <= 2 && Vector3.Distance(transform.position, _currentTargetPosition) <= stopDistance)
                {
                        _zAnimController.Attack = true;
                }
                else
                {
                        _zAnimController.Attack = false;
                }

                transform.LookAt(_currentPath.corners[1]);
            }

            yield return new WaitForSeconds(refreshTime);
        }
    }

    private IEnumerator GoTowardsTargetPosition()
    {
        _goingTowardsRandomSound = true;

        while (true)
        {
            NavMesh.CalculatePath(transform.position, _currentTargetPosition, -1, _currentPath);

            if (_currentPath.corners.Length > 0)
            {
                AI_Helper.DebugPath(_currentPath,Color.yellow);

                if (_currentPath.corners.Length <= 2 && Vector3.Distance(transform.position, _currentTargetPosition) <= stopDistance)
                {

                    _goingTowardsRandomSound = false;
                    ResetTarget();

                    yield break;
                }

                transform.LookAt(_currentPath.corners[1]);
            }

            yield return new WaitForSeconds(refreshTime);
        }
    }


    #region methods
    private void ResetTarget()
    {
        _currentTargetPosition = Vector3.zero;
        _currentTargetPriority = TargetPriorityLevel.none;
    }

    #region PlayerViewMethods

    void GetPlayersNextToTheZombie(List<GameObject> players)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, DetectPlayersAroundR);
        if (colliders.Length != 0)
        {
            Vector3 targetDirection = Vector3.zero;
            float targetDistance = 0f;

            foreach (Collider target in colliders)
            {
                if (target.CompareTag("Player"))
                {
                    targetDirection = (target.transform.position - transform.position).normalized;
                    targetDistance = Vector3.Distance(transform.position, target.transform.position);
                    bool nothingBetween = !Physics.Raycast(transform.position, targetDirection, targetDistance, _obstructViewLayers);

                    if (nothingBetween)
                    {
                        players.Add(target.gameObject);
                    }
                    
                }
            }
        }
    }

    void GetPlayersInsideFOV(List<GameObject> players) 
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position - transform.forward * FovOffset, FovRange);
        if(colliders.Length != 0) 
        {
            foreach (Collider target in colliders) 
            {
                if (target.CompareTag("Player")) 
                {
                    if (Vector3.Distance(transform.position, target.transform.position) > DetectPlayersAroundR) // to not repeat the same ones from the other list
                    {
                        Vector3 targetDirection = (target.transform.position - transform.position).normalized;
                        bool insideRange = Vector3.Angle(transform.forward, targetDirection) < FovViewAngle / 2 ;

                        if (insideRange) 
                        {
                            float targetDistance = Vector3.Distance(transform.position, target.transform.position);
                            bool nothingBetween = !Physics.Raycast(transform.position, targetDirection, targetDistance, _obstructViewLayers);

                            if (nothingBetween) 
                            {
                                players.Add(target.gameObject);
                            }
                        }
                    }

                }
            }
        }
    }

    Vector3 ClosestPlayerPosition(List<GameObject> players) 
    {
        Vector3 _closestPlayerPosition =Vector3.positiveInfinity;

        for (int i = 0; i < players.Count; i++)
        {
            if (Vector3.Distance(transform.position, players[i].transform.position) < Vector3.Distance(transform.position, _closestPlayerPosition)) 
            {
                _closestPlayerPosition = players[i].transform.position;
            }
        }

        return _closestPlayerPosition;
    }

    #endregion

    #region TargetValidationMethods

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

    #endregion

    #endregion


    #region Gizmos 
    private void OnDrawGizmos()
    {
        Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -FovViewAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, FovViewAngle / 2);

        Gizmos.color = Color.black;
        Vector3 originPos = transform.position - transform.forward * FovOffset;
        Gizmos.DrawLine(originPos, originPos + viewAngle01 * FovRange);
        Gizmos.DrawLine(originPos, originPos + viewAngle02 * FovRange);

        Gizmos.DrawWireSphere( transform.position, DetectPlayersAroundR);
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion

    
}