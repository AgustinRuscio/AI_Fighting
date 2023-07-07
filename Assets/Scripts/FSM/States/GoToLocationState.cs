using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLocationState : States
{
    private Transform _agentPosition;

    private AiAgent _aiAgent;

    private LayerMask _obstaclesMask;
    private LayerMask _enemyMask;

    private Vector3 _target;

    public GoToLocationState SetAgent(AiAgent agent)
    {
        _aiAgent = agent;
        _agentPosition = agent.transform;

        return this;
    }

    public GoToLocationState SetLayers(LayerMask obstacleLayer, LayerMask enemyLayer)
    {
        _obstaclesMask = obstacleLayer;
        _enemyMask = enemyLayer;
        return this;
    }

    public override void OnStart(params object[] parameters)
    {
        _target = (Vector3)parameters[0] ;

        Debug.Log(_aiAgent.name + " Entro a Go to location");

        if (!Tools.InLineOfSight(_agentPosition.position, _target, _obstaclesMask))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _target, true, false);
        
    }

    public override void OnStop()
    {
        Debug.Log(_aiAgent.name + " Salio a Go to location");
    }



    public override void Update()
    {
        if (_aiAgent.GetClosestEnemy() != Vector3.zero)
        {
            if (Tools.FieldOfView(_aiAgent.transform.position, _aiAgent.transform.forward, _aiAgent.GetClosestEnemy(), _aiAgent._viewRadius, _aiAgent._viewAngle, _enemyMask))
                finiteStateMach.ChangeState(StatesEnum.Fight, _aiAgent.GetCurrentEnemy(), true);
        }

        _aiAgent.ApplyForce(_aiAgent.Seek(_target));

        if (Vector3.Distance(_agentPosition.position, _target) < 1)
            finiteStateMach.ChangeState(StatesEnum.Idle);
    }
}