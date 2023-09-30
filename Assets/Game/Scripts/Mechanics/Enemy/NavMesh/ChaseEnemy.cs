using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseEnemy : MonoBehaviour
{
    private NavMeshPath _path;
    private bool _hasPath;
    private NavMeshAgent _agent;
    private Queue<Vector3> _cornerQueue;
    private Vector3 _currentDestination;
    private Vector3 _targetPoint;
    private Vector3 _direction;
    private float _currentDistance;

    void OnEnable()
    {
        InitVars();
        CalculateNavMesh();

        SetupPath(_path);
    }

    private void CalculateNavMesh()
    {
        _agent.CalculatePath(_targetPoint, _path);
    }

    private void InitVars()
    {
        _targetPoint = PlayerController.Instance.transform.position; // Set target point here
        _agent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
    }

    void SetupPath(NavMeshPath path)
    {
        _cornerQueue = new Queue<Vector3>();
        foreach (var corner in path.corners)
        {
            _cornerQueue.Enqueue(corner);
        }

        GetNextCorner();
        _hasPath = true;
    }

    private void GetNextCorner()
    {
        if (_cornerQueue.Count > 0)
        {
            _currentDestination = _cornerQueue.Dequeue();
            _direction = _currentDestination - transform.position;
            _hasPath = true;
        }
        else
        {
            _hasPath = false;
        }
    }

    void FixedUpdate()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (_hasPath)
        {
            _currentDistance = Vector3.Distance(transform.position, _currentDestination);

            if (_currentDistance > 1)
                transform.position += _direction * 0.4f * Time.deltaTime;
            else
                GetNextCorner();
        }
    }
}
