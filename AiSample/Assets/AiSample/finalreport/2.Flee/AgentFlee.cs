using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentFlee : MonoBehaviour
{
    [SerializeField]
    private Transform _agent = null;

    private Vector3 _pickPos = Vector3.zero;

    private Vector3 _fleePos = Vector3.zero;

    [SerializeField]
    private float _maxSpeed = 1.0f;

    private Vector3 _velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

        if (Input.GetMouseButtonUp(1))
        {
            Vector3 mouse_pos = Input.mousePosition;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), -Vector3.up, out hit, 1000))
            {
                _fleePos = hit.point;
            }
        }

        if (Vector3.Distance(_agent.transform.position, _fleePos) < 15.0f)
        {
            _velocity = _velocity + (flee(_fleePos) * Time.deltaTime);
        }

        _velocity = _velocity + (seek(_pickPos) * Time.deltaTime);

        _agent.transform.position = _agent.transform.position + _velocity;

        _agent.transform.forward = _velocity.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_pickPos, 1.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_fleePos, 5.0f);
    }

    private Vector3 seek(Vector3 target_pos)
    {
        Vector3 desired_velocity = ((target_pos - _agent.transform.position).normalized) * _maxSpeed;

        desired_velocity.y = 0.0f;

        return (desired_velocity - _velocity);
    }

    private Vector3 flee(Vector3 target_pos)
    {
        Vector3 desired_velocity = ((_agent.transform.position - target_pos).normalized) * _maxSpeed;
        
        desired_velocity.y = 0.0f;

        return (desired_velocity - _velocity);
    }
}
