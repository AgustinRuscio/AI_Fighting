using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAgent : AiAgent, IBoid
{
    public Vector3 GetBoidPosition() => transform.position;

    [SerializeField]
    private Transform _leaderTransform;

    private void Start()
    {
        AddBoidToHash();

        //_fsm.AddState(StatesEnum.GoToLocation, new GoToLocationState().SetAgent(this).SetObstacleLayer(_obstaclesMask));
        //_fsm.AddState(StatesEnum.LeaderFollowing, new LeadFollowState().SetObstacleLayer(_obstaclesMask).SetLeaderTransform(_leaderTransform).SetAget(this));
        //_fsm.AddState(StatesEnum.PathFinding, new PathfindingState(this).SetLayers(_nodeMask,_obstaclesMask));
    }

    override protected void Update()
    {
        base.Update();

        //_fsm.Update();
        ApplyForce(LeaderFollowing(_leaderTransform.position, GameManager.instance.allBoids));
        ObstacleAvoidanceLogic();
    }

    public void AddBoidToHash()=> GameManager.instance.AddBoid(this);


    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(transform.position, _arriveRadius);
        Gizmos.DrawWireSphere(transform.position, _separationRadius);
    }
}
