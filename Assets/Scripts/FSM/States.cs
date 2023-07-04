//-------------------------
//          Agustin Ruscio
//-------------------------



public abstract class States
{
    public FiniteStateMachine finiteStateMach;

    public abstract void OnStart(params object[] parameters);

    public abstract void Update();

    public abstract void OnStop();
}