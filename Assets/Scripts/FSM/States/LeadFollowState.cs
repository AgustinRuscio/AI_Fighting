using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadFollowState : States
{
    private AiAgent _agent;
    private BoidAgent _boideAgent;

    private Vector3 _leaderPosition;

    private LayerMask _obstacleLayer;
    private LayerMask _enemyMask;

    public LeadFollowState SetAget(AiAgent agent,BoidAgent boid)
    {
        _agent = agent;
        _boideAgent = boid;
        return this;
    }
    public LeadFollowState SetLeaderTransform(Transform leaderPosition)
    {
        //_leaderPosition = leaderPosition.position;
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

        _leaderPosition = _boideAgent.GetLeaderPosition();
    }

    public override void OnStop()
    {
        Debug.Log("Salgo LeadFollow");

    }

    public override void Update()
    {
        if (_agent.WinCheck())
            finiteStateMach.ChangeState(StatesEnum.Dance, false);
        

        if (Vector3.Distance(_leaderPosition, _boideAgent.GetLeaderPosition()) > 3)
            _leaderPosition = _boideAgent.GetLeaderPosition();

        if (_agent.GetClosestEnemy() != Vector3.zero)
        {
            if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetClosestEnemy(), _agent._viewRadius, _agent._viewAngle, _enemyMask))
                finiteStateMach.ChangeState(StatesEnum.Fight, _agent.GetCurrentEnemy(), false);
        }

        if (!Tools.InLineOfSight(_agent.transform.position, _leaderPosition, _obstacleLayer))
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _leaderPosition, false, false);


        if (Vector3.Distance(_agent.transform.position, _leaderPosition) < 3)
        {
            Debug.Log("Im too close");
            Vector3 lastFwd = _agent.transform.forward;
            _agent.StopMovement();
            _agent.transform.forward = lastFwd;
        }
        else
            _agent.ApplyForce(_agent.LeaderFollowing(_leaderPosition, GameManager.instance.allBoids));

            _agent.ApplyForce(_agent.Separation(GameManager.instance.allBoids));
    }
}