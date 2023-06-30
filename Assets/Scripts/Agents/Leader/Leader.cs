using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : AiAgent
{
    [SerializeField]
    private TeamEnum _team;

    private void Start()
    {
        _fsm.AddState(StatesEnum.Idle, new IdleState().SetAgent(this).SetLayer(_enemiesMask));
        _fsm.AddState(StatesEnum.GoToLocation, new GoToLocationState().SetAgent(this).SetObstacleLayer(_obstaclesMask));
        _fsm.AddState(StatesEnum.PathFinding, new PathfindingState(this).SetLayers(_nodeMask,_obstaclesMask));
    }

    protected override void Update()
    {
        base.Update();

        _fsm.Update();

        ObstacleAvoidanceLogic();

        if (Input.GetMouseButtonDown(0) && _team == TeamEnum.RedTeam)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Floor"))
                    _fsm.ChangeState(StatesEnum.GoToLocation, hit.point);
            }
        }

        if (Input.GetMouseButtonDown(1) && _team == TeamEnum.BlueTeam)
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