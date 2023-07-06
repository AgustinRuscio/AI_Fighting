using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : States
{
    private AiAgent _agent;

    public DeathState(AiAgent agent)
    {
        _agent = agent;
    }   

    public override void OnStart(params object[] parameters)
    {
        _agent.StopMovement();
    }

    public override void OnStop() { }

    public override void Update() { }
}
