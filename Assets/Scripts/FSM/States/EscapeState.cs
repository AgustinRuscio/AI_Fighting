using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState : States
{
    private AiAgent _agent;

    private LayerMask _obstacleMask;

    private Vector3 _escapeDirection;

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
        Debug.Log(_agent.name + "Entro en Escape");
        _escapeDirection = new Vector3(Random.Range(-36, 21), 0, Random.Range(-35, 22));
    }

    public override void OnStop()
    {
        Debug.Log(_agent.name + "Salgo del Escape");
    }

    public override void Update()
    {
        if (!Tools.InLineOfSight(_agent.transform.position, _escapeDirection, _obstacleMask))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _escapeDirection);
        
        _agent.ApplyForce(_agent.Seek(_escapeDirection));

        if(Vector3.Distance(_agent.transform.position, _escapeDirection) <= 1)
            finiteStateMach.ChangeState(StatesEnum.Idle);
    }
}
