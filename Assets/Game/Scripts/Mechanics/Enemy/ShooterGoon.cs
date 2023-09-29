using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterGoon : GoonBase
{
    [SerializeField] float _shootInterval = 3f;
    [SerializeField] float _bulletVelocity = 12f;
    [SerializeField] float _bulletLifeTime = 5f;
    [SerializeField] float _bulletScaleAmount = 0.98f;

    private BulletPool _bulletPool;
    private ShootCombatAbility _shootAbility;
    private float _shootTimeStamp = 0f;

    protected override void Awake()
    {
        base.Awake();
        _bulletPool = GetComponentInChildren<BulletPool>();

        if (_bulletPool == null)
        {
            Debug.LogError("There is no bullet pool on this object or its children.");
        }
    }

    private void Start()
    {
        // Establishing the enemy's ability
        _shootAbility = new ShootCombatAbility(_bulletPool, _bulletVelocity, _bulletLifeTime, _bulletScaleAmount);
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= _shootTimeStamp + _shootInterval)
        {
            Shoot();
            _shootTimeStamp = Time.time;
        }
    }

    public void Shoot()
    {
        OnCombatAbility?.Invoke();
        _shootAbility.UseAbility();
    }
}
