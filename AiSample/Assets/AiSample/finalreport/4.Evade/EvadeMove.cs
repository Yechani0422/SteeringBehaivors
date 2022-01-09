using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeMove : MonoBehaviour
{
    [SerializeField]
    private Transform _target = null;

    [SerializeField]
    private Transform _agent = null;

    [SerializeField]
    private Transform _spawner = null;

    [SerializeField]
    private float _maxSpeed = 0.1f;

    private Vector3 _velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(_agent.transform.position, _target.position) < 10.0f)
        {
            _velocity = _velocity + evade(_target);

            
        }

        _agent.transform.position = _agent.transform.position + _velocity;

        _agent.transform.forward = _velocity.normalized;

        if (Vector3.Distance(_agent.transform.position,_spawner.position)>40.0f)
        {
            _agent.transform.position = _spawner.position;
        }
    }

    private Vector3 evade(Transform target_agent)
    {
        Vector3 targetVelocity = target_agent.gameObject.GetComponent<AgentMove1>()._velocity;
        
        float targetSpeed = target_agent.gameObject.GetComponent<AgentMove1>()._maxSpeed;
        
        Vector3 distance = (target_agent.position - _agent.transform.position).normalized;
        
        float updateesAhead = distance.sqrMagnitude / (_maxSpeed + targetSpeed);
        
        Vector3 futurePosition = target_agent.position + (targetVelocity * updateesAhead);

        return flee(futurePosition);
    }

    private Vector3 flee(Vector3 target_pos)
    {
        Vector3 desired_velocity = ((_agent.transform.position - target_pos).normalized) * _maxSpeed;

        desired_velocity.y = 0.0f;

        return (desired_velocity - _velocity);
    }
}
