using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : States
{
    private Transform _agentTransform;

    private AiAgent _agent;

    private LayerMask _enemyMask;

    public IdleState SetAgent(AiAgent agent)
    {
        _agent = agent;
        _agentTransform = agent.transform;

        return this;
    }
    public IdleState SetLayer(LayerMask enemiesLayer)
    {
        _enemyMask = enemiesLayer;

        return this;
    }

    public override void OnStart(params object[] parameters)
    {
        Debug.Log("Idle");

        _agent.StopMovement();

        if (Tools.FieldOfView(_agentTransform.position, _agentTransform.forward, _agent.GetClosestEnemy(), _agent._viewRadius, _agent._viewAngle, _enemyMask))
                finiteStateMach.ChangeState(StatesEnum.Fight, _agent.GetClosestEnemy());
    }

    public override void OnStop() { }
    public override void Update() { }
}