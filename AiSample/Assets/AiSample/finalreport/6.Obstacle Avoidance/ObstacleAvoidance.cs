using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [SerializeField]
    private Transform _agent = null;

    private Vector3 _pickPos = Vector3.zero;

    private Vector3 _velocity = Vector3.zero;

    [SerializeField]
    private float _deceleration = 1.0f;

    [SerializeField]
    private float _maxSpeed = 10.0f;

    [SerializeField]
    private float _minAhead= 5.0f;

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
        
        _velocity = _velocity + arrive(_pickPos);

        _velocity = _velocity + avoidance();

        _agent.transform.position = _agent.transform.position + (_velocity*Time.deltaTime);

        _agent.transform.forward = _velocity.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pickPos, 1.0f);
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

    private Vector3 avoidance()
    {
        Ray ray = new Ray(_agent.transform.position, _agent.transform.forward);
       
        RaycastHit hit;

        Vector3 avoidanceForce = Vector3.zero;

        float aheadLength = _minAhead + (_velocity.magnitude / _maxSpeed) * _minAhead;
       //Debug.Log(aheadLength);

        float lateralForce = 0.0f;

        float brakeForce = 0.2f;

        float obstacleRadius = 0.0f;

        if (Physics.Raycast(ray,out hit , aheadLength))
        {
            if(hit.collider.tag=="obstacle")
            {
                Debug.Log(hit.transform.position);              

                Vector3 obstacleLocalPos=_agent.transform.InverseTransformVector(hit.transform.position);
                Debug.Log(obstacleLocalPos);

                lateralForce = 1.0f * (aheadLength - obstacleLocalPos.x) / aheadLength;

                obstacleRadius = hit.transform.localScale.x / 2;
                Debug.Log(obstacleRadius);

                avoidanceForce.x = (obstacleRadius - obstacleLocalPos.x) * brakeForce;
                avoidanceForce.z = (obstacleRadius - obstacleLocalPos.z) * lateralForce;
                
                //float dynamic_length = _velocity.magnitude / _maxSpeed;
                //Vector3 ahead = _agent.transform.position + _velocity.normalized * dynamic_length;
                //avoidanceForce = ahead - obstacleLocalPos;
                //avoidanceForce = avoidanceForce.normalized * 10.0f;
            }
            else
            {
                avoidanceForce = Vector3.zero;
            }
        }

        avoidanceForce.y=0;
        
        Debug.DrawRay(_agent.transform.position, _agent.transform.forward * aheadLength, Color.red);
        return avoidanceForce;
    }
}
