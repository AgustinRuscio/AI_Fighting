using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState : States
{
    private AiAgent _agent;

    private LayerMask _obstacleMask;

    private Vector3 _escapeDirection;

    private bool _isLeader;

    public EscapeState SetAgent(AiAgent agent)
    {
        _agent = agent;
        return this;
    }
    public EscapeState SetLayer(LayerMask obstacleLayer)
    {
        _obstacleMask = obstacleLayer;
        return this;
    }


    public override void OnStart(params object[] parameters)
    {
        _isLeader = (bool)parameters[0];
        
        _escapeDirection = new Vector3(Random.Range(-15, 45), 0, Random.Range(-37, 19));
    }

    public override void OnStop()
    {
        Debug.Log(_agent.name + "Salgo del Escape");
    }

    public override void Update()
    {
        if (!Tools.InLineOfSight(_agent.transform.position, _escapeDirection, _obstacleMask))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _escapeDirection, _isLeader);
        
        _agent.ApplyForce(_agent.Seek(_escapeDirection));

        if(Vector3.Distance(_agent.transform.position, _escapeDirection) <= 1)
        {
            if (_isLeader)
                finiteStateMach.ChangeState(StatesEnum.Idle);
            else
                finiteStateMach.ChangeState(StatesEnum.GoToLocation);
        }
    }
}
