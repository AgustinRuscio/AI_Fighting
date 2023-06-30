using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLocationState : States
{
    private Transform _agentPosition;

    private AiAgent _aiAgent;

    private LayerMask _obstaclesMask;

    private Vector3 _target;

    public GoToLocationState SetAgent(AiAgent agent)
    {
        _aiAgent = agent;
        _agentPosition = agent.transform;

        return this;
    }

    public GoToLocationState SetObstacleLayer(LayerMask obstacleLayer)
    {
        _obstaclesMask = obstacleLayer;

        return this;
    }

    public override void OnStart(params object[] parameters)
    {
        _target = (Vector3)parameters[0];

        if (!Tools.InLineOfSight(_agentPosition.position, _target, _obstaclesMask))
        {
            Debug.Log("cambio");
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _target);
        }
    }

    public override void OnStop()
    {
        
    }



    public override void Update()
    {
        _aiAgent.ApplyForce(_aiAgent.Seek(_target));

        Debug.Log("yendo");

        if (Vector3.Distance(_agentPosition.position, _target) < 1)
            finiteStateMach.ChangeState(StatesEnum.Idle);
    }
}