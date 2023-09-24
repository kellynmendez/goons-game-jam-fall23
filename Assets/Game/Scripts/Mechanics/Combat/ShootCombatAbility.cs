using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCombatAbility : ICombatAbility
{
    
    public Vector3 Position { get; set; }
    
    private BulletPool _bulletPool;
    private float _velocity;
    private float _lifeTime;
    private float _scaleAmount;

    public ShootCombatAbility(BulletPool bulletPool, Vector3 position, float velocity, float lifeTime, float scaleAmount)
    {
        
        Position = position;
        _bulletPool = bulletPool;
        _velocity = velocity;
        _lifeTime = lifeTime;
        _scaleAmount = scaleAmount;
    }

    public void UseAbility()
    {
        if ( _bulletPool == null )
        {
            Debug.LogError("No bullet pool was given.");
            return;
        }

        GameObject bulletGameObj = _bulletPool.GetPooledBullet();

        if (bulletGameObj != null)
        {
            Bullet bullet = bulletGameObj.GetComponent<Bullet>();
            bullet.Activate(Position, _bulletPool.transform.forward, _velocity, _lifeTime, _scaleAmount);
        }
    }
}
