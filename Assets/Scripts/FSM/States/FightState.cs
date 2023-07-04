using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightState : States
{
    private AiAgent _agent;

    private AiAgent _enemy;

    private LayerMask _enemyMask;

    public FightState SetAgent(AiAgent agent)
    {
        _agent = agent;
        return this;
    }

    public FightState SetLayers(LayerMask enemyMask)
    {
        _enemyMask = enemyMask;
        return this;
    }

    public override void OnStart(params object[] parameters)
    {
        Debug.Log(_agent.name + " Entro a fight");

        _agent._viewComponent.FightMode(true);
        _enemy = (AiAgent)parameters[0];
    }

    public override void OnStop()
    {
        _agent._viewComponent.FightMode(false);
        Debug.Log(_agent.name + " Salio de fight");
    }

    public override void Update()
    {
        _agent.ApplyForce(_agent.Seek(_enemy.transform.position));

        if (Vector3.Distance(_agent.transform.position, _enemy.transform.position) < 1)
        {
            _agent.StopMovement();

            _agent.transform.forward = _enemy.transform.position - _agent.transform.position;

            if (_agent._timer.CheckCoolDown())
            {
                _agent._viewComponent.Punch();
            }
        }


        if (!_enemy.IsAlive())
            finiteStateMach.ChangeState(StatesEnum.Idle);

        if (!Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetClosestEnemy(), _agent._viewRadius, _agent._viewAngle, _enemyMask))
            finiteStateMach.ChangeState(StatesEnum.Idle);

    }
}