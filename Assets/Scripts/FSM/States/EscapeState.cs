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
        
        _escapeDirection = new Vector3(Random.Range(-17, 8), 0, Random.Range(-29f, 8f));
        Debug.Log(_escapeDirection);
    }

    public override void OnStop()
    {
        Debug.Log(_agent.name + "Salgo del Escape");
    }

    public override void Update()
    {
        if (_agent.WinCheck())
            finiteStateMach.ChangeState(StatesEnum.Dance, _isLeader);
        

        if (!Tools.InLineOfSight(_agent.transform.position, _escapeDirection, _obstacleMask))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _escapeDirection, _isLeader, false);
        
        _agent.ApplyForce(_agent.Seek(_escapeDirection));
        _agent.ApplyForce(_agent.Separation(GameManager.instance.allBoids) * _agent._separationRadius);

        if (Vector3.Distance(_agent.transform.position, _escapeDirection) <= 1)
        {
            if (_isLeader)
                finiteStateMach.ChangeState(StatesEnum.Idle);
            else
                finiteStateMach.ChangeState(StatesEnum.GoToLocation);
        }
    }
}
