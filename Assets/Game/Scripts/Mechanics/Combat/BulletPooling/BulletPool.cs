using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] int _poolSize = 6;

    private List<GameObject> _bullets;
    private bool _IsPlayersPool;

    private void Awake()
    {
        PlayerController player = transform.parent.GetComponent<PlayerController>();
        if (player)
        {
            _IsPlayersPool = true;
        }
        else
        {
            GoonBase goon = transform.parent.GetComponent<GoonBase>();
            if (goon)
            {
                _IsPlayersPool = false;
            }
            else
            {
                Debug.LogError("Bullet pool must be on an instance of either the player or a goon.");
            }
        }

        _bullets = new List<GameObject>();
    }

    void Start()
    {
        // Create all the bullets in the pool
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject newBullet = Instantiate(_bulletPrefab);
            newBullet.GetComponent<Bullet>().SetIsPlayersBullet(_IsPlayersPool);
            newBullet.SetActive(false);
            // Add to pool
            _bullets.Add(newBullet);
        }
    }

    public GameObject GetPooledBullet()
    {
        // If a game object in the pool is not active, give it out
        foreach (GameObject bullet in _bullets)
        {
            if (!bullet.activeSelf)
            {
                bullet.GetComponent<Bullet>().SetIsPlayersBullet(_IsPlayersPool);
                bullet.SetActive(true);
                return bullet;
            }
        }
        // If all are active
        return null;
    }
}
