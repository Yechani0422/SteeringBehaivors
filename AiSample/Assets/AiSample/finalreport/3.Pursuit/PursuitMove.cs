using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitMove : MonoBehaviour
{
    [SerializeField]
    private Transform _target = null;

    [SerializeField]
    private Transform _agent = null;

    [SerializeField]
    private float _maxSpeed = 0.2f;
    

    private bool _pursuitStart = false;

    private Vector3 _velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _pursuitStart = true;
        }

        if (_pursuitStart)
        {
            _velocity = _velocity + pursuit(_target);

            _agent.transform.position = _agent.transform.position + _velocity;

            _agent.transform.forward = _velocity.normalized;
        }
    }

    private Vector3 pursuit(Transform target_agent)
    {    
        //추격대상 가속도
        Vector3 targetVelocity = target_agent.gameObject.GetComponent<AgentMove>()._velocity;

        //추격대상 최고속도
        float targetSpeed = target_agent.gameObject.GetComponent<AgentMove>()._maxSpeed;

        //추격대상까지의 거리
        Vector3 distance = (target_agent.position - _agent.transform.position).normalized;

        //예측 시간 = 추격 대상까지의 거리 / (추격자 최고속도+ 추격 대상 최고속도)
        float T = distance.sqrMagnitude / (_maxSpeed+targetSpeed);

        //추격자의 미래 위치 = 추격자의 현위치 + (추격자의 가속도 * 시간)
        Vector3 futurePosition = target_agent.position + (targetVelocity * T);

        return seek(futurePosition);
    }

    private Vector3 seek(Vector3 target_pos)
    {
        Vector3 desired_velocity = ((target_pos - _agent.transform.position).normalized) * _maxSpeed;

        desired_velocity.y = 0.0f;

        return (desired_velocity - _velocity);
    }
}
