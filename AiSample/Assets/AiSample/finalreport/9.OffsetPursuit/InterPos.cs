using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterPos : MonoBehaviour
{
    [SerializeField]
    private Transform _agent = null;

    [SerializeField]
    private Transform _agentA = null;

    [SerializeField]
    private Transform _agentB = null;

    private leaderMove vel_A = null;

    private leaderMove vel_B = null;

    private Vector3 _velocity = Vector3.zero;

    [SerializeField]
    private float _deceleration = 1.0f;

    [SerializeField]
    private float _maxSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        vel_A = GameObject.Find("AgentA").GetComponent<leaderMove>();

        vel_B = GameObject.Find("AgentB").GetComponent<leaderMove>();
    }

    // Update is called once per frame
    void Update()
    {
        _velocity = _velocity + interpose(_agentA.transform.position,_agentB.transform.position);

        _agent.transform.position = _agent.transform.position + (_velocity * Time.deltaTime);

        _agent.transform.forward = _velocity.normalized;
    }

    private Vector3 arrive(Vector3 target_pos)
    {
        float distance = Vector3.Distance(target_pos, _agent.transform.position);

        if (distance > 0.0f)
        {
            Vector3 to_target = target_pos - _agent.transform.position;

            float _speed = distance / _deceleration;

            _speed = Mathf.Min(_speed, _maxSpeed);

            Vector3 desired_velocity = to_target / distance * _speed;

            desired_velocity.y = 0;

            return (desired_velocity - _velocity);
        }

        return Vector3.zero;
    }

    private Vector3 interpose(Vector3 a_pos, Vector3 b_pos)
    {
        //1. 다른 두 에이전트의 현재 위치에서의 중간지점을 계산한다.
        Vector3 MidPoint = (a_pos + b_pos) / 2.0f;

        //2. 에이전트의 현재 위치에서 중간지점까지 도달하는데 걸리는 시간(T)을 계산한다.
        float TimeToReachMidPoint = Vector3.Distance(_agent.transform.position, MidPoint) / _maxSpeed;

        //3. T시간 동안 다른 두 에이전트가 이동할 위치를 계산한다.
        Vector3 APos = a_pos + vel_A._velocity * TimeToReachMidPoint;
        Vector3 BPos = b_pos + vel_B._velocity * TimeToReachMidPoint;

        //4. 3에서 계산된 두 위치의 중간 지점을 계산한다.
        MidPoint = (APos + BPos) / 2.0f;

        //5. 에이전트가 4의 위치로 이동한다.
        return arrive(MidPoint);
    }
}