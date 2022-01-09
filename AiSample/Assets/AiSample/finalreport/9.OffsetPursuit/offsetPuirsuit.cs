using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offsetPuirsuit : MonoBehaviour
{
    [SerializeField]
    private Transform _agent = null;

    private leaderMove _Leader = null;

    private Vector3 _velocity = Vector3.zero;

    [SerializeField]
    private float _deceleration = 1.0f;

    [SerializeField]
    private float _maxSpeed = 10.0f;

    [SerializeField]
    private float offSetPosX = 0.0f;

    [SerializeField]
    private float offSetPosY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _Leader = GameObject.Find("AgentA").GetComponent<leaderMove>();
    }

    // Update is called once per frame
    void Update()
    {
        _velocity = _velocity+offsetPursuit();

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

    private Vector3 offsetPursuit()
    {
        Vector3 localOffsetPos = Vector3.zero;

        Vector3 WorldOffsetPos = _Leader.transform.TransformPoint(_Leader._offSet + new Vector3(offSetPosX, 0.0f, offSetPosY));
        
       

        Vector3 ToOffset = WorldOffsetPos - _agent.transform.position;

        float LookAheadTime = ToOffset.magnitude / (_maxSpeed + _Leader._velocity.magnitude);

        Vector3 desired_velocity = WorldOffsetPos + (_Leader._velocity * LookAheadTime);

        return arrive(desired_velocity);
    }
}
