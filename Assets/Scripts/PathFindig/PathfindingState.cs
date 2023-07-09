//-------------------------
//          Agustin Ruscio
//-------------------------


using System.Collections.Generic;
using UnityEngine;

public class PathfindingState : States
{
    private List<Vector3> _path;

    private Node startNode;
    private Node goalNode;

    private AiAgent _agent;

    private LayerMask _nodeLayer;

    private LayerMask _obstacleMask;
    private LayerMask _enemyMask;

    private float _viewRadius;

    private Vector3 _parameterPos;

    private bool _isLeader;
    private bool _cameFromDancing;

    #region Builder

    public PathfindingState(AiAgent agent)
    {
        _agent = agent;
        _viewRadius = _agent._viewRadius;
    }

    public PathfindingState SetLayers(LayerMask nodeMask, LayerMask obstacles, LayerMask enemyLayer)
    {
        _nodeLayer = nodeMask;
        _obstacleMask = obstacles;
        _enemyMask = enemyLayer;
        return this;
    }

    #endregion

    public override void OnStart(params object[] parameters)
    {
        Debug.Log(_agent.name + " Entro al pathFinding");

        _parameterPos = (Vector3)parameters[0];

        Debug.Log(_parameterPos);

        _isLeader = (bool)parameters[1];
        _cameFromDancing = (bool)parameters[2];

        goalNode = GetNode(_parameterPos);

        startNode = GetNode(_agent.transform.position);

        _path = ThetaStar(startNode, goalNode);

        _path.Reverse();
    }

    public Node GetNode(Vector3 initPos)
    {
        var nearNode = Physics.OverlapSphere(initPos, _viewRadius, _nodeLayer);

        Node nearestNode = null;

        float distance = 900000;

        for (int i = 0; i < nearNode.Length; i++)
        {
            if (Tools.InLineOfSight(initPos, nearNode[i].transform.position, _obstacleMask))
            {
                RaycastHit hit;

                Vector3 dir = nearNode[i].transform.position - initPos;


                if (Physics.Raycast(initPos, dir, out hit))
                {
                    if (hit.distance < distance)
                    {
                        distance = hit.distance;
                        nearestNode = nearNode[i].gameObject.GetComponent<Node>();
                    }
                }
            }
        }

        return nearestNode;
    }

    public override void Update()
    {
        if (_agent.WinCheck())
            finiteStateMach.ChangeState(StatesEnum.Dance, _isLeader);
        

        if (_agent.GetClosestEnemy() != Vector3.zero)
        {
            if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetClosestEnemy(), _agent._viewRadius, _agent._viewAngle, _enemyMask) & _cameFromDancing)
                finiteStateMach.ChangeState(StatesEnum.Fight, _agent.GetCurrentEnemy(), _isLeader);
        }
        

        if (Tools.InLineOfSight(_agent.transform.position, goalNode.transform.position, _obstacleMask) && !_cameFromDancing)
            finiteStateMach.ChangeState(StatesEnum.GoToLocation, _parameterPos);

        if (_path.Count > 0)
        {
            if(Tools.InLineOfSight(_agent.transform.position, _path[0], _obstacleMask))
                MovethroughNodes();
            else
            {
                //----Recalculate path
                goalNode = GetNode(_parameterPos);

                startNode = GetNode(_agent.transform.position);

                _path = ThetaStar(startNode, goalNode);

                _path.Reverse();
            }
        }
        else
        {
            if(_cameFromDancing)
                finiteStateMach.ChangeState(StatesEnum.Dance, _isLeader);
            else
            {
                if(_isLeader)
                    finiteStateMach.ChangeState(StatesEnum.Idle);
                else
                    finiteStateMach.ChangeState(StatesEnum.GoToLocation);
            }
        }

        if(!_isLeader)
            _agent.ApplyForce(_agent.Separation(GameManager.instance.allBoids) * _agent._separationRadius);
    }

    private void MovethroughNodes()
    {
        _agent.ApplyForce(_agent.Seek(_path[0]));

        if (Vector3.Distance(_agent.transform.position, _path[0]) <= 1)
            _path.RemoveAt(0);
    }

    public List<Vector3> AStar(Node startingNode, Node endNode)
    {
        if(startingNode == null || endNode == null) return new List<Vector3>();

        PriorityQueue<Node> frontier = new PriorityQueue<Node>();

        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        Node current = null;

        while(frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == endNode) break;

            foreach(var next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.Cost;

                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Enqueue(next, newCost + Heuristic(next, endNode));
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                }
                else if(newCost < costSoFar[current])
                {
                    frontier.Enqueue(next, newCost + Heuristic(next, endNode));
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                }
            }
        }

        if (current != endNode) return new List<Vector3>();

        List<Vector3> path = new List<Vector3>();

        while(current != startingNode)
        {
            path.Add(current.transform.position);
            current = cameFrom[current];
        }

        path.Add(startingNode.transform.position); 

        return path;
    }

    public List<Vector3> ThetaStar(Node start, Node end)
    {
        var path = AStar(start, end);
        if (path == null || path.Count == 0) return path;

        var completedPath = new List<Vector3>();
        completedPath.AddRange(path);

        int current = 0;

        while (current < completedPath.Count)
        {
            if (current + 2 >= completedPath.Count) break;


            if (Tools.InLineOfSight(completedPath[current], completedPath[current + 2], _obstacleMask))
                completedPath.RemoveAt(current + 1);
            else
                current++;
        }

        return completedPath;
    }

    private float Heuristic(Node start, Node End) => (End.transform.position - start.transform.position).sqrMagnitude;
    
    public override void OnStop() { Debug.Log(_agent.name + " Salio del pathFinding"); }
}