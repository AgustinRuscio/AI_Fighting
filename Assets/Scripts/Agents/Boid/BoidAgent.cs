using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAgent : AiAgent, IBoid
{
    public Vector3 GetBoidPosition() => transform.position;

    [SerializeField]
    private Transform _leaderTransform;

    public float _leaderMaxDistance;

    protected override void Start()
    {
        base.Start();

        AddBoidToHash();

        _fsm.AddState(StatesEnum.GoToLocation, new LeadFollowState().SetLayers(_wallMask, _enemiesMask).SetLeaderTransform(_leaderTransform).SetAget(this, this));
        _fsm.AddState(StatesEnum.PathFinding, new PathfindingState(this).SetLayers(_nodeMask, _wallMask, _enemiesMask));
        _fsm.AddState(StatesEnum.Fight, new FightState().SetAgent(this).SetLayers(_enemiesMask));
        _fsm.AddState(StatesEnum.Escape, new EscapeState().SetAgent(this).SetLayer(_wallMask));
        _fsm.AddState(StatesEnum.Dance, new DanceState().SetAgent(this));
    }

    override protected void Update()
    {
        if (_alive && !_defeat)
        {
            base.Update();

            _fsm.Update();
        }
    }

    public void AddBoidToHash()=> GameManager.instance.AddBoid(this);

    public Vector3 GetLeaderPosition() => _leaderTransform.position;
}