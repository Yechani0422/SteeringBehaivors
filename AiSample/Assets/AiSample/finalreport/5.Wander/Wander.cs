using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    private Vector3 _pickPos = Vector3.zero;

    private Vector3 _velocity = Vector3.zero;

    private Vector3 _wanderTarget = Vector3.zero;

    private Vector3 _circleCenter = Vector3.zero;

    private Vector3 _target = Vector3.zero;

    private float _jitterTime = 0.0f;

    private float _angle = 0.0f;

    [SerializeField]
    private float _wanderRadius = 2.0f;

    [SerializeField]
    private float _wanderDis = 2.0f;

    [SerializeField]
    private Transform _agent = null;

    [SerializeField]
    private float _maxSpeed = 0.1f;

    [SerializeField]
    private Transform _reSpawner = null;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        _jitterTime += Time.deltaTime;

        //에이전트가 화면밖으로 나가면 리스폰
        if (Vector3.Distance(_agent.transform.position, _reSpawner.position) > 45.0f)
        {
            _agent.transform.position = _reSpawner.position;

            _velocity = Vector3.zero;
        }

        _velocity = _velocity + (wander() * Time.deltaTime);

        _agent.transform.position = _agent.transform.position + _velocity;

        _agent.transform.forward = _velocity.normalized;
    }

    private Vector3 wander()
    {
        //1frame 이동할거리 계산  
        if (_jitterTime >= 1.0f)
        {
            _angle = Random.Range(0, 360) * Mathf.Deg2Rad;
            _jitterTime = 0.0f;
        }
        Debug.Log(Random.Range(0, 360) * Mathf.Deg2Rad);
        Debug.Log(_angle);
        //대상의 위치를 1frame 이후의 위치로 이동
        _wanderTarget += new Vector3(_wanderRadius * Mathf.Cos(_angle), 0.0f, _wanderRadius * Mathf.Sin(_angle));

        //wanderTarget.x = _radius * Mathf.Cos(angle);
        //wanderTarget.z = _radius * Mathf.Sin(angle);

        // 대상의 위치를 원둘레로 변환
        // 원의 지름이 1이므로 정규화 하는 것으로 원둘레에 위치하게 됨.
        _wanderTarget.Normalize();

        // 원의 지름이 1보다 큰 경우에 대한 처리.
        _wanderTarget *= _wanderRadius;

        // 원을 전방으로 이동.
        _circleCenter = _wanderTarget + new Vector3(0.0f, 0.0f, _wanderDis);

        //원을 월드좌표로 변환
        _target = transform.TransformPoint(_circleCenter);

        //대상방향으로 이동하는 힘
        Vector3 desired_velocity = ((_target - _agent.transform.position).normalized) * _maxSpeed;

        desired_velocity.y = 0.0f;

        return desired_velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_target, 0.3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_agent.transform.position + _agent.transform.forward * _wanderDis, _wanderRadius);
    }

}
