using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightState : States
{
    private AiAgent _agent;

    private AiAgent _enemy;

    private LayerMask _enemyMask;

    private bool _isLeader;

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

        _isLeader = (bool)parameters[1];

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
                _agent._viewComponent.Punch();
            
        }

        
        if (!_enemy.IsAlive())
        {
            if (_agent.WinCheck())
                finiteStateMach.ChangeState(StatesEnum.Dance, _isLeader);
            else
            {
                if (_isLeader)
                finiteStateMach.ChangeState(StatesEnum.Idle);
                else
                finiteStateMach.ChangeState(StatesEnum.GoToLocation);
            }
        }

        
        if (!Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _enemy.transform.position, _agent._viewRadius, _agent._viewAngle, _enemyMask))
        {
            _enemy = null;

            if(_agent.GetClosestEnemy() == Vector3.zero)
            {
                if (_agent.WinCheck())
                    finiteStateMach.ChangeState(StatesEnum.Dance, _isLeader);
                else
                {
                    if (_isLeader)
                        finiteStateMach.ChangeState(StatesEnum.Idle);
                    else
                        finiteStateMach.ChangeState(StatesEnum.GoToLocation);
                }
            }
            else
            {
                _enemy = _agent.GetCurrentEnemy();

                if(Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _enemy.transform.position, _agent._viewRadius, _agent._viewAngle, _enemyMask))
                    _agent.transform.LookAt(_enemy.transform.position);
            }
        }
    }
}