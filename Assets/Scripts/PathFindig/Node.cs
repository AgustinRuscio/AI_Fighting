//------------------------------//
//     Made by Agustin Ruscio   //
//------------------------------//


using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] [Range(1, 5)]
    private int _cost = 1;

    [SerializeField]
    private float _neighborRadius;

    [SerializeField]
    private LayerMask _nodeMask;

    [SerializeField]
    private LayerMask _obstableMask;

    [SerializeField]
    private bool GizmosActivated;

    public int Cost { get { return _cost; }}

    public List<Node> GetNeighbors()
    {
        List<Node> neighborsLits = new List<Node>();

        var obteinNodes = Physics.OverlapSphere(transform.position, _neighborRadius, _nodeMask);

        for (int i = 0; i < obteinNodes.Length; i++)
        {
            if (obteinNodes[i] == this.GetComponent<Collider>()) continue;

            if (Tools.InLineOfSight(transform.position, obteinNodes[i].transform.position, _obstableMask))
            {
                Node _currentNode = obteinNodes[i].GetComponent<Node>();
                neighborsLits.Add(_currentNode);
            }
        }

        return neighborsLits;
    }

    private void OnDrawGizmos()
    {
        if (GizmosActivated)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _neighborRadius);
        }
    }
}