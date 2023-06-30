using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadFollowState : States
{
    private AiAgent _agent;

    private Transform _leaderPosition;
    private LayerMask _obstacleLayer;

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
    public LeadFollowState SetObstacleLayer(LayerMask obstacleLayer)
    {
        _obstacleLayer = obstacleLayer;
        return this;
    }


    public override void OnStart(params object[] parameters)
    {
        if (!Tools.InLineOfSight(_agent.transform.position, _leaderPosition.position, _obstacleLayer))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _leaderPosition);
    }

    public override void OnStop()
    {
        

    }

    public override void Update()
    {
        if (!Tools.InLineOfSight(_agent.transform.position, _leaderPosition.position, _obstacleLayer))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _leaderPosition);

        _agent.ApplyForce(_agent.LeaderFollowing(_leaderPosition.position, GameManager.instance.allBoids));
    }
}