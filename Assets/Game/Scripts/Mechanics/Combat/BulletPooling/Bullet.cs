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
    private Vector3 _initialForward;
    private bool _isPlayersBullet;

    public void SetIsPlayersBullet(bool isPlayersBullet)
    {
        _isPlayersBullet = isPlayersBullet;
    }

    public void Activate(Vector3 initialPosition, Vector3 initialForward, float velocity, float lifeTime)
    {
        _velocity = velocity;
        _lifeTime = lifeTime;
        _activationTimeStamp = Time.time;
        this.enabled = true;

        transform.position = initialPosition;
        _initialForward = initialForward;
        transform.forward = _initialForward;

        OnActivate?.Invoke();
    }

    void Update()
    {
        // While bullet is within its lifetime, move it forward at a constant velocity
        if (Time.time - _activationTimeStamp > _lifeTime)
        {
            Deactivate();
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * _velocity);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPlayersBullet && other.CompareTag("Goon"))
        {
            GoonBase goon = other.gameObject.GetComponent<GoonBase>();
            goon?.Hurt();
            Deactivate();
        }
        else if (!_isPlayersBullet && other.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player?.Hurt(this.transform.forward);
            Deactivate();
        }
        else if (!(other.CompareTag("Player")) && !(other.CompareTag("Goon")))
        {
            Deactivate();
        }
    }
}