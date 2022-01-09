using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agent : MonoBehaviour
{
    private Vector3 _pickPos = Vector3.zero;

    public Vector3 _velocity = Vector3.zero;

    private Vector3 _wanderTarget=Vector3.zero;

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

    [SerializeField]
    private float _feelerLength = 5.0f;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouse_pos = Input.mousePosition;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), -Vector3.up, out hit, 1000))
            {
                _pickPos = hit.point;
            }
        }

        _jitterTime += Time.deltaTime;

        //에이전트가 화면밖으로 나가면 리스폰
        if (Vector3.Distance(_agent.transform.position, _reSpawner.position) > 60.0f)
        {
            _agent.transform.position = _reSpawner.position;

            _velocity = Vector3.zero;
        }

        _velocity = _velocity + (wander() * Time.deltaTime)+wallAvoidance();

        _agent.transform.position = _agent.transform.position + _velocity;

        _agent.transform.forward = _velocity.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_pickPos, 1.0f);
        Gizmos.DrawWireSphere(_target, 0.3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_agent.transform.position+_agent.transform.forward*_wanderDis, _wanderRadius);
    }

    private Vector3 wander()
    {
        //1frame 이동할거리 계산  
        if (_jitterTime>=1.0f)
        {
            _angle = Random.Range(0, 360) * Mathf.Deg2Rad;
            _jitterTime = 0.0f;
        }
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
        _target= transform.TransformPoint(_circleCenter);

        //대상방향으로 이동하는 힘
        Vector3 desired_velocity = ((_target - _agent.transform.position).normalized) *_maxSpeed;

        desired_velocity.y = 0.0f;

        return desired_velocity;
    }

    private Vector3 wallAvoidance()
    {
        //정면 더듬이
        Ray frontFeeler = new Ray(_agent.transform.position, _agent.transform.forward);
        //왼쪽 더듬이
        Ray leftFeeler = new Ray(_agent.transform.position, Quaternion.AngleAxis(-45.0f,transform.up)*_agent.transform.forward);
        //오른쪽 더듬이
        Ray rightFeeler = new Ray(_agent.transform.position, Quaternion.AngleAxis(45.0f, transform.up) * _agent.transform.forward);

        RaycastHit hit;

        Vector3 avoidanceForce = Vector3.zero;
       
        //정면 더듬이 조종힘 계산
        if (Physics.Raycast(frontFeeler, out hit, _feelerLength))
        {
            if (hit.collider.tag == "wall")
            {               
                Vector3 feelerVertex = _agent.transform.position + _agent.transform.forward * _feelerLength; //더듬이 끝의 위치
                Vector3 overShoot = feelerVertex - hit.point;        //(더듬이 끝 위치-벽과 더듬이의 교점)
                avoidanceForce = hit.transform.right * overShoot.sqrMagnitude; //조종힘= 벽의 법선벡터* 더듬이가 벽을뚫은 길이
            }
            else
            {
                avoidanceForce = Vector3.zero;
            }
        }
        //왼쪽 더듬이 조종힘 계산
        else if (Physics.Raycast(leftFeeler, out hit, _feelerLength/2))
        {
            if (hit.collider.tag == "wall")
            {
                Vector3 feelerVertex = _agent.transform.position + (Quaternion.AngleAxis(-45.0f, transform.up) * _agent.transform.forward) * (_feelerLength/2); //더듬이 끝의 위치
                Vector3 overShoot = feelerVertex - hit.point;        //(더듬이 끝 위치-벽과 더듬이의 교점)
                avoidanceForce = hit.transform.right * overShoot.sqrMagnitude; //조종힘= 벽의 법선벡터* 더듬이가 벽을뚫은 길이
            }
            else
            {
                avoidanceForce = Vector3.zero;
            }
        }
        //오른쪽 더듬이 조종힘 계산
        else if (Physics.Raycast(rightFeeler, out hit, _feelerLength/2))
        {
            if (hit.collider.tag == "wall")
            {
                Vector3 feelerVertex = _agent.transform.position + (Quaternion.AngleAxis(45.0f, transform.up) * _agent.transform.forward) * (_feelerLength/2); //더듬이 끝의 위치
                Vector3 overShoot = feelerVertex - hit.point;        //(더듬이 끝 위치-벽과 더듬이의 교점)
                avoidanceForce = hit.transform.right * overShoot.sqrMagnitude; //조종힘= 벽의 법선벡터* 더듬이가 벽을뚫은 길이
            }
            else
            {
                avoidanceForce = Vector3.zero;
            }
        }

        avoidanceForce.y = 0;


        //더듬이 gizmo
        Debug.DrawRay(_agent.transform.position, _agent.transform.forward * _feelerLength, Color.green);

        Debug.DrawRay(_agent.transform.position, Quaternion.AngleAxis(-45.0f, transform.up) * _agent.transform.forward * _feelerLength/2, Color.green);

        Debug.DrawRay(_agent.transform.position, Quaternion.AngleAxis(45.0f, transform.up) * _agent.transform.forward * _feelerLength/2, Color.green);


        return avoidanceForce;
    }
}
