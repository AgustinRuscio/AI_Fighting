//------------------------------//
//     Made by Agustin Ruscio   //
//------------------------------//


using UnityEngine;

public class PunchZone : MonoBehaviour
{
    private float _damage;

    [SerializeField]
    private AiAgent _aiAgent;

    private void OnTriggerEnter(Collider other)
    {
        AiAgent punched = other.gameObject.GetComponent<AiAgent>();

        _damage = Random.Range(10, 20);

        if (punched != null && punched != _aiAgent  && _aiAgent._timer.CheckCoolDown())
        {
            punched.TakeDamage(_damage);
            _aiAgent._timer.ResetTimer();
        }
    }
}