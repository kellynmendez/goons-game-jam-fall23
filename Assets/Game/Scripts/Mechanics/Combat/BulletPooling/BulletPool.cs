using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] int _poolSize = 6;

    private List<GameObject> _bullets;

    private void Awake()
    {
        _bullets = new List<GameObject>();
    }

    void Start()
    {
        // Create all the bullets in the pool
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject newBullet = Instantiate(_bulletPrefab);
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
                bullet.SetActive(true);
                return bullet;
            }
        }
        // If all are active
        Debug.Log("no more bullets");
        return null;
    }
}
