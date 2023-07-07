using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : AiAgent
{
    protected override void Start()
    {
        base.Start();

        _fsm.AddState(StatesEnum.Idle, new IdleState().SetAgent(this, transform).SetLayer(_enemiesMask));
        _fsm.AddState(StatesEnum.GoToLocation, new GoToLocationState().SetAgent(this).SetLayers(_wallMask, _enemiesMask));
        _fsm.AddState(StatesEnum.PathFinding, new PathfindingState(this).SetLayers(_nodeMask,_wallMask, _enemiesMask));
        _fsm.AddState(StatesEnum.Fight, new FightState().SetAgent(this).SetLayers(_enemiesMask));
        _fsm.AddState(StatesEnum.Escape, new EscapeState().SetAgent(this).SetLayer(_wallMask));
        _fsm.AddState(StatesEnum.Dance, new DanceState().SetAgent(this));
    }

    protected override void Update()
    {
        if (_alive) 
        { 
            base.Update();
        
            _fsm.Update();

            if (Input.GetMouseButtonDown(0) && IsAlive() &&_team == TeamEnum.RedTeam && GameManager.instance.SimulationOn())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Floor"))
                        _fsm.ChangeState(StatesEnum.GoToLocation, hit.point);
                }
            }

            if (Input.GetMouseButtonDown(1) && IsAlive() && _team == TeamEnum.BlueTeam && GameManager.instance.SimulationOn())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Floor"))
                        _fsm.ChangeState(StatesEnum.GoToLocation, hit.point);
                }
            }
        }
    }
}