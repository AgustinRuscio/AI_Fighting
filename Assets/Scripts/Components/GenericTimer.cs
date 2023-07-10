//------------------------------//
//     Made by Agustin Ruscio   //
//------------------------------//


using UnityEngine;

public class GenericTimer
{
    private float _timer;

    private float _coolDown;

    public GenericTimer(float _coolDown) => this._coolDown = _coolDown;
   

    public void RunTimer() => _timer = _timer + 1 * Time.deltaTime;

    public bool CheckCoolDown()
    {
        if(_timer > _coolDown)
            return true;   
        else 
            return false;
    }

    public void ResetTimer() => _timer = 0;
}