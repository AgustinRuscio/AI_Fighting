using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidIdleState : States
{
    private AiAgent _agent;
    private BoidAgent _boideAgent;

    private Vector3 _leaderTarget;

    private LayerMask _obstacleMask;
    private LayerMask _enemyMask;

    public BoidIdleState SetAgent(AiAgent agent, BoidAgent boid)
    {
        _agent = agent;
        _boideAgent = boid;
        return this;
    }

    public BoidIdleState SetLayers(LayerMask obstacleLayer, LayerMask enemyLayer)
    {
        _obstacleMask = obstacleLayer;
        _enemyMask = enemyLayer;
        return this;
    }

    public BoidIdleState SetTarget(Transform leaderTransform)
    {
        _leaderTarget = leaderTransform.position;
        return this;
    }

    public override void OnStart(params object[] parameters)
    {
        
        
        Debug.Log("Leader position: " + _leaderTarget);
        
        Debug.Log("Idle");
        
        _agent.StopMovement();
        
        

    }

    public override void OnStop()
    {
        
    }

    public override void Update()
    {
        //if (_agent.GetClosestEnemy() != Vector3.zero)
        //{
        //    if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetClosestEnemy(), _agent._viewRadius, _agent._viewAngle, _enemyMask))
        //    {
        //        Debug.Log("Lets Fight");
        //        finiteStateMach.ChangeState(StatesEnum.Fight, _agent.GetCurrentEnemy());
        //    }
        //}
        
    }
}
