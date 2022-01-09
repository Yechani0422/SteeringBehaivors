using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMove1 : MonoBehaviour
{

    [SerializeField]
    private Transform _agent = null;

    private Vector3 _pickPos = Vector3.zero;

    [SerializeField]
    public float _maxSpeed = 1.0f;

    public Vector3 _velocity = Vector3.zero;

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

        _velocity = _velocity + (seek(_pickPos) * Time.deltaTime);

        _agent.transform.position = _agent.transform.position + _velocity;

        _agent.transform.forward = _velocity.normalized;
    }

    private Vector3 seek(Vector3 target_pos)
    {
        Vector3 desired_velocity = ((target_pos - _agent.transform.position).normalized) * _maxSpeed;

        desired_velocity.y = 0.0f;

        return (desired_velocity - _velocity);
    }
}
