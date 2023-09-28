using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] UnityEvent OnActivate = null;
    
    private float _activationTimeStamp = 0f;
    private float _scaleAmount;
    private float _velocity;
    private float _lifeTime;
    private Vector3 _startScale;
    private Vector3 _initialForward;
    private bool _isPlayersBullet;

    private void OnEnable()
    {
        _startScale = gameObject.transform.localScale;
        //StartCoroutine(FadeToInactive());
    }

    public void SetIsPlayersBullet(bool isPlayersBullet)
    {
        _isPlayersBullet = isPlayersBullet;
    }

    public void Activate(Vector3 initialPosition, Vector3 initialForward, float velocity, float lifeTime, float scaleAmount)
    {
        _velocity = velocity;
        _lifeTime = lifeTime;
        _scaleAmount = scaleAmount;
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

    private void Deactivate()
    {
        gameObject.transform.localScale = _startScale;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthSystem health = other.gameObject.GetComponent<HealthSystem>();

        if (health != null)
        {
            // If this is the player shooting and they hit a goon OR this is the enemy shooting and they hit the player
            if ( (_isPlayersBullet && other.CompareTag("Goon")) || (!_isPlayersBullet && other.CompareTag("Player")) )
            {
                Debug.Log($"{other.gameObject.name} was hurt");
                health.Hurt();
                Deactivate();
            }
        }/*
        else if (!other.CompareTag("Player"))
        {
            Deactivate();
        }*/
    }

    private IEnumerator FadeToInactive()
    {
        float start = Time.time;

        while (start + _lifeTime > Time.time)
        {
            // Slowly shrink object
            gameObject.transform.localScale = gameObject.transform.localScale * _scaleAmount;
            yield return new WaitForSeconds(0.1f); // scale consistently across any machine
        }
        Debug.Log("set bullet inactive");
        gameObject.transform.localScale = _startScale;
        gameObject.SetActive(false);
        yield break;
    }
}