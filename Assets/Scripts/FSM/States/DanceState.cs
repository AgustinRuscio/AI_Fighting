using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceState : States
{
    private AiAgent agent;

    private int ondeDance;

    public DanceState SetAgent(AiAgent agent)
    {
        this.agent = agent;
        return this;
    }

    public override void OnStart(params object[] parameters)
    {
        Debug.Log("Entro al Dance");
        ondeDance = 0;
        agent.StopMovement();

    }

    public override void OnStop() { }

    public override void Update()
    {
        if(ondeDance == 0)
        {
            agent.SetDanceMode();
            ondeDance = 1;
        }
        
    }
}