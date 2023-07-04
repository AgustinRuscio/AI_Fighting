using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : States
{
    private Transform _agentTransform;

    private AiAgent _agent;

    private LayerMask _enemyMask;

    public IdleState SetAgent(AiAgent agent, Transform transform)
    {
        _agent = agent;
        _agentTransform = transform;

        return this;
    }
    public IdleState SetLayer(LayerMask enemiesLayer)
    {
        _enemyMask = enemiesLayer;

        return this;
    }

    public override void OnStart(params object[] parameters)
    {
        Debug.Log(_agent.name + " Entro al Idle");
        _agent.StopMovement();
    }

    public override void OnStop()
    {
        Debug.Log(_agent.name + " salio del Idle");
    }

    public override void Update() 
    {
        if (_agent.GetClosestEnemy() != Vector3.zero)
        {
            if (Tools.FieldOfView(_agentTransform.position, _agentTransform.forward, _agent.GetClosestEnemy(), _agent._viewRadius, _agent._viewAngle, _enemyMask))
                finiteStateMach.ChangeState(StatesEnum.Fight, _agent.GetCurrentEnemy());
        }
    }
}