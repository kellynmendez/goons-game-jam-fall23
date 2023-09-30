using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuttPuttAbilityKill : MonoBehaviour
{
    private ParticleSystem _particles;
    private List<ParticleCollisionEvent> _collisionEvents;
    private bool checkedForCollisions = false;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        _collisionEvents = new List<ParticleCollisionEvent>();
    }
    private void OnParticleCollision(GameObject other)
    {
        if (!checkedForCollisions)
        {
            checkedForCollisions = true;
            int numCollisionEvents = _particles.GetCollisionEvents(other, _collisionEvents);

            bool collided = false;

            for (int i = 0; i < numCollisionEvents && !collided; i++)
            {
                GoonBase goon = _collisionEvents[i].colliderComponent.gameObject.GetComponent<GoonBase>();
                if (goon != null)
                {
                    collided = true;
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
}
