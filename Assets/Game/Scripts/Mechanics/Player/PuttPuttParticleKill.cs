using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuttPuttParticleKill : MonoBehaviour
{
    private ParticleSystem _particles;
    private List<ParticleCollisionEvent> _collisionEvents;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        _collisionEvents = new List<ParticleCollisionEvent>();
    }
    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = _particles.GetCollisionEvents(other, _collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            GoonBase goon = _collisionEvents[i].colliderComponent.gameObject.GetComponent<GoonBase>();
            if (goon != null)
            {
                goon.Hurt();
            }

            Bullet bullet = _collisionEvents[i].colliderComponent.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Deactivate();
            }
        }
    }
}
