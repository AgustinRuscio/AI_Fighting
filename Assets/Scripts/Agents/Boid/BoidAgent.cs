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

        _fsm.AddState(StatesEnum.GoToLocation, new LeadFollowState().SetLayers(_wallMask, _enemiesMask).SetLeaderTransform(_leaderTransform).SetAget(this, this));
        _fsm.AddState(StatesEnum.PathFinding, new PathfindingState(this).SetLayers(_nodeMask, _wallMask, _enemiesMask));
        _fsm.AddState(StatesEnum.Fight, new FightState().SetAgent(this).SetLayers(_enemiesMask));
        _fsm.AddState(StatesEnum.Escape, new EscapeState().SetAgent(this).SetLayer(_wallMask));
        _fsm.AddState(StatesEnum.Death, new DeathState(this));
    }

    override protected void Update()
    {
        if (_alive)
        {
            base.Update();

            _fsm.Update();
        }
    }

    public void AddBoidToHash()=> GameManager.instance.AddBoid(this);

    public Vector3 GetLeaderPosition() => _leaderTransform.position;
    

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(transform.position, _arriveRadius);
        Gizmos.DrawWireSphere(transform.position, _separationRadius);
    }
}
