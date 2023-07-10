//------------------------------//
//     Made by Agustin Ruscio   //
//------------------------------//


using UnityEngine;
using UnityEngine.UI;

public class AgentView 
{
    private Animator _animator;

    private Slider _slider;

    public AgentView(Animator anim, Slider slider)
    {
        _animator = anim;
        _slider = slider;   
    }

    public void Death() => _animator.SetTrigger("Death");
    public void Punch() => _animator.SetTrigger("Punch");
    public void GetHit() => _animator.SetTrigger("GetHit");

    public void WinDance(int index) 
    { 
        _animator.SetTrigger("WinDance");
        _animator.SetFloat("DanceNum", index);

    }

    public void FightMode(bool mode) => _animator.SetBool("FightMode", mode);
    public void InjuredMode(bool mode) => _animator.SetBool("InjuredMode", mode);
    public void LoseMode() => _animator.SetTrigger("Lose");
    public void Movement(float move) => _animator.SetFloat("Move", move);

    public void UpdateHud(float life, float maxLife) => _slider.value = life / maxLife;
}