using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadFollowState : States
{
    private AiAgent _agent;

    private Transform _leaderPosition;
    private LayerMask _obstacleLayer;
    private LayerMask _enemyMask;

    public LeadFollowState SetAget(AiAgent agent)
    {
        _agent = agent;
        return this;
    }
    public LeadFollowState SetLeaderTransform(Transform leaderPosition)
    {
        _leaderPosition = leaderPosition;
        return this;
    }
    public LeadFollowState SetLayers(LayerMask obstacleLayer, LayerMask enemyLayer)
    {
        _obstacleLayer = obstacleLayer;
        _enemyMask = enemyLayer;
        return this;
    }


    public override void OnStart(params object[] parameters)
    {
        Debug.Log("Estado LeadFollow");
        Debug.Log(_agent.name + " Agente");
        Debug.Log(_obstacleLayer.ToString() + " Mask");
        Debug.Log(_leaderPosition.position + " Leaderpos");

        if (!Tools.InLineOfSight(_agent.transform.position, _leaderPosition.position, _obstacleLayer))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _leaderPosition.position);
    }

    public override void OnStop()
    {
        Debug.Log("Salgo LeadFollow");

    }

    public override void Update()
    {
        if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetClosestEnemy(), _agent._viewRadius, _agent._viewAngle, _enemyMask))
            finiteStateMach.ChangeState(StatesEnum.Fight, _agent.GetCurrentEnemy());

        if (!Tools.InLineOfSight(_agent.transform.position, _leaderPosition.position, _obstacleLayer))
           finiteStateMach.ChangeState(StatesEnum.PathFinding, _leaderPosition.position);

        _agent.ApplyForce(_agent.LeaderFollowing(_leaderPosition.position, GameManager.instance.allBoids));
    }
}