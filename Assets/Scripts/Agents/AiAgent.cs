//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;

public abstract class AiAgent : MonoBehaviour
{
    [SerializeField]
    private bool GizmosActivated;

    protected FiniteStateMachine _fsm = new FiniteStateMachine();

    [Header("Fight Atributs")]

    [SerializeField]
    protected float _life;

    [SerializeField]
    protected float _damage;

    [SerializeField]
    protected float _coolDown;

    private GenericTimer _timer;

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
    protected LayerMask _obstaclesMask;

    [SerializeField]
    protected LayerMask _enemiesMask;

    public float _viewRadius;

    [HideInInspector]
    public float _viewAngle = 90;

    [Header("Flocking")]

    [SerializeField]
    protected float _separationRadius;

    [SerializeField]
    protected float _arriveRadius;

    [SerializeField]
    protected Vector3 _offset;

    [Header("Avoidancec")]

    [SerializeField]
    protected float _avoidanceRadius;

    [SerializeField]
    protected LayerMask _avoidanceMask;


    private void Awake() => _timer = new GenericTimer(_coolDown);
    

    protected virtual void Update() 
    {
        Move();
        _timer.RunTimer();
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

    private Vector3 Separation(HashSet<IBoid> boids)
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
        Vector3 separationForce = Separation(boids);
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

    public void StopMovement()
    {
        _velocity = Vector3.zero;
    }

    #endregion

    protected void ObstacleAvoidanceLogic()
    {
        if (Physics.Raycast((transform.position + new Vector3(0, 1, 0)) + transform.right / 2, transform.forward, _avoidanceRadius, _avoidanceMask))
        {
            ApplyForce(CalculateSteering(-transform.right * _speed));
            //Debug.Log("Obstacle R");
        }
        else if (Physics.Raycast((transform.position + Vector3.up) - transform.right / 2, transform.forward, _avoidanceRadius, _avoidanceMask))
        {
            ApplyForce(CalculateSteering(transform.right * _speed));
            //Debug.Log("Obstacle L");
        }
            //Debug.Log("No Obstacle");
    }

    public Vector3 GetClosestEnemy()
    {
        List<Collider> list = new List<Collider>();
        Collider[] enemies = Physics.OverlapSphere(transform.position, _viewRadius);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == transform.gameObject.GetComponent<Collider>()) continue;

            list.Add(enemies[i]);
        }

        if(list.Count == 0)
            return Vector3.zero;

        float dist = 90000;

        Vector3 closestEnemy = Vector3.zero;

        for (int i = 0; i < list.Count; i++)
        {
            Vector3 currentEnemy = list[i].gameObject.GetComponent<Transform>().position;

            if (Vector3.Distance(transform.position, currentEnemy) < dist)
                closestEnemy = currentEnemy;
        }

        return closestEnemy;
    }

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