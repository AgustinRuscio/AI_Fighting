//-------------------------
//          Agustin Ruscio
//-------------------------


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class AiAgent : MonoBehaviour
{
    [SerializeField]
    private bool GizmosActivated;

    protected FiniteStateMachine _fsm = new FiniteStateMachine();

    [SerializeField]
    private Animator _animator;

    public AgentView _viewComponent;

    [SerializeField]
    protected TeamEnum _team;

    [SerializeField]
    private bool _isLeader;

    [Header("Fight Atributs")]

    [SerializeField]
    protected float _life;

    protected float _coolDown;

    private bool _alive = true;

    private bool _injured = true;

    [SerializeField]
    private Slider _healthBar;

    [SerializeField]
    private GameObject _punchArea;

    public GenericTimer _timer;

    private AiAgent _currentEnemy;

    //-----Atributs
    protected Vector3 _velocity;

    [Header("Movement")]
    [SerializeField] [Range(1, 10)]
    protected float _maxForce;

    [SerializeField]
    private float _speed;

    [Header("FOV & LOS")]

    [SerializeField]
    protected LayerMask _nodeMask;

    [SerializeField]
    protected LayerMask _wallMask;

    [SerializeField]
    protected LayerMask _enemiesMask;

    public float _viewRadius;

    [HideInInspector]
    public float _viewAngle = 90;

    [SerializeField]
    private float _obstacleAvoidanceMultiplayer;

    [Header("Flocking")]

    [SerializeField]
    protected float _separationRadius;

    [SerializeField]
    protected float _arriveRadius;

    [SerializeField]
    private float _separationMultiplayer;

    [SerializeField]
    protected Vector3 _offset;

    [Header("Avoidancec")]

    [SerializeField]
    protected float _avoidanceRadius;

    [SerializeField]
    protected LayerMask _avoidanceMask;


    private void Awake()
    {
        _coolDown = Random.Range(1.5f, 4);

        _timer = new GenericTimer(_coolDown);
        _viewComponent = new AgentView(_animator, _healthBar);

        _viewComponent.UpdateHud(_life);
    }
    

    protected virtual void Update() 
    {
        Move();
        _timer.RunTimer();
        ObstacleAvoidanceLogic();
        _viewComponent.Movement(_velocity.magnitude);
    }

    #region STEERING_BEHAVIOR

    public void Move()
    {
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
    }

    public Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;

        desired.Normalize();

        desired *= _speed;

        return CalculateSteering(desired);
    }

    private Vector3 Arrive(Vector3 arriveTarget)
    {
        float dist = Vector3.Distance(transform.position, arriveTarget);

        if (dist > _arriveRadius)
            return Seek(arriveTarget);

        Vector3 desired = arriveTarget - transform.position;

        desired.Normalize();

        desired *= _speed * (dist / _arriveRadius);

        return desired;
    }

    public Vector3 Separation(HashSet<IBoid> boids)
    {
        Vector3 desired = Vector3.zero;

        foreach (var boid in boids)
        {
            Vector3 dir = boid.GetBoidPosition() - transform.position;

            if (dir.magnitude <= _separationRadius)
                desired += dir;
        }

        if (desired == Vector3.zero) return desired;

        desired = desired.normalized * _speed * -1;

        return desired;
    }
    public Vector3 LeaderFollowing(Vector3 leaderPosition, HashSet<IBoid> boids)
    {
        float dist = Vector3.Distance(transform.position, leaderPosition);

        if (dist > _arriveRadius)
            return Seek(leaderPosition);

        Vector3 separationForce = Separation(boids) * _separationMultiplayer;
        Vector3 arriveForce = Arrive(leaderPosition + _offset);

        Vector3 desiredVelocity = separationForce + arriveForce;

        return CalculateSteering(desiredVelocity);
    }

    private Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);

        return steering;
    }

    public void ApplyForce(Vector3 force)
    {
        force.y = 0;
        _velocity = Vector3.ClampMagnitude(_velocity + force, _speed);
    }

    public void StopMovement() => _velocity = Vector3.zero;

    public Vector3 InvertDirection() => -_velocity;
    

    #endregion

    public bool IsAlive() => _alive;
    public void TakeDamage(float damage)
    {
        _life -= damage;

        _viewComponent.UpdateHud(_life);

        if (_life <= 20)
            InjuredMode();

        if(_life <= 0)
            Death();

        if(_fsm.CurrentState() != StatesEnum.Fight && _fsm.CurrentState() != StatesEnum.Escape)
        {
            GetClosestEnemy();
            _fsm.ChangeState(StatesEnum.Fight, GetCurrentEnemy(), _isLeader);
        }
    }

    private void InjuredMode()
    {
        _injured = true;
        _viewComponent.InjuredMode(_injured);

        _fsm.ChangeState(StatesEnum.Escape, _isLeader);
    }

    public void OnPunch()
    {
        _punchArea.SetActive(true);
    }

    public void OffPunch()
    {
        _punchArea.SetActive(false);
    }

    public void Death()
    {
        _alive = false;
        _viewComponent.Death();
    }

    public float GetLife() => _life;

    protected void ObstacleAvoidanceLogic()
    {
        if (Physics.Raycast((transform.position + new Vector3(0, 1, 0)) + transform.right / 2, transform.forward, _avoidanceRadius, _avoidanceMask))
        {
            ApplyForce(CalculateSteering(-transform.right * _speed) * _obstacleAvoidanceMultiplayer);
            //Debug.Log("Obstacle R");
        }
        else if (Physics.Raycast((transform.position + Vector3.up) - transform.right / 2, transform.forward, _avoidanceRadius, _avoidanceMask))
        {
            ApplyForce(CalculateSteering(transform.right * _speed) * _obstacleAvoidanceMultiplayer);
            //Debug.Log("Obstacle L");
        }
            //Debug.Log("No Obstacle");
    }

    public TeamEnum GetTeam() => _team;

    public Vector3 GetClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, _viewRadius, _enemiesMask);

        if(enemies.Length == 0 ) return Vector3.zero;

        float dist = 90000;

        Vector3 closestEnemy = Vector3.zero;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == transform.gameObject.GetComponent<Collider>()) continue;

            if (!Tools.InLineOfSight(transform.position, enemies[i].transform.position, _wallMask)) continue;

            var currentCheck = enemies[i].gameObject.GetComponent<AiAgent>();

            if (currentCheck ==  null) continue;
            
            if(!currentCheck.IsAlive()) continue;

            TeamEnum check = currentCheck.GetTeam();

            if (check == _team) continue;

            if (Vector3.Distance(transform.position, enemies[i].transform.position) < dist)
            {
                closestEnemy = enemies[i].transform.position;
                dist = closestEnemy.magnitude;
                _currentEnemy = currentCheck;
            }
        }

         if(closestEnemy == Vector3.zero) return Vector3.zero;

        return closestEnemy;
    }

    public AiAgent GetCurrentEnemy() => _currentEnemy;
    

    #region DrawGizmos
    Vector3 DirFromAngel(float angleInDegrees) => new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));


    protected virtual void OnDrawGizmos()
    {
        if (GizmosActivated)
        {    
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _viewRadius);
            
            Vector3 dirA = DirFromAngel(_viewAngle / 2 + transform.eulerAngles.y);
            Vector3 dirB = DirFromAngel(-_viewAngle / 2 + transform.eulerAngles.y);
            
            Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * _viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * _viewRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _viewRadius);
            
            //----------------Obstacle Avoidance
            Gizmos.color = Color.magenta;

            Vector3 orpos = (transform.position + Vector3.up) + transform.right / 2;

            Gizmos.DrawLine(orpos, orpos + transform.forward * _avoidanceRadius);

            Vector3 o2rpos = (transform.position + Vector3.up) - transform.right / 2;

            Gizmos.DrawLine(o2rpos, o2rpos + transform.forward * _avoidanceRadius);
        }
    }
    #endregion
}