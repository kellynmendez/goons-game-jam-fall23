using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuttPuttParticleKill : MonoBehaviour
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
                PlayerController player = _collisionEvents[i].colliderComponent.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    Debug.Log("player damaged");
                    collided = true;
                    player.Hurt(transform.parent.transform.forward);
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
