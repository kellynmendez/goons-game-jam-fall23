using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] UnityEvent OnActivate = null;
    
    private float _activationTimeStamp = 0f;
    private float _velocity;
    private float _lifeTime;
    private Vector3 _startForwardDir;
    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        //StartCoroutine(FadeToInactive());
    }

    public void Activate(Vector3 initialPosition, Vector3 forward, float velocity, float lifeTime)
    {
        _velocity = velocity;
        _lifeTime = lifeTime;
        _activationTimeStamp = Time.time;
        this.enabled = true;

        transform.position = initialPosition;
        _startForwardDir = forward;
        transform.forward = _startForwardDir;

        OnActivate?.Invoke();
    }

    void FixedUpdate()
    {
        if (Time.time - _activationTimeStamp > _lifeTime)
        {
            Deactivate();
        }
        else
        {
            _rigidBody.velocity = _startForwardDir * _velocity;
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}