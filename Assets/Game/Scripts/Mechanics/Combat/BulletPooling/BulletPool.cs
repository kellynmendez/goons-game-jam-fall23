using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] int _poolSize = 6;

    private List<GameObject> _bullets;
    private bool _IsPlayersPool;
    private Collider _thisObjsCollider;

    private void Awake()
    {
        PlayerController player = transform.parent.GetComponent<PlayerController>();
        _bullets = new List<GameObject>();

        if (player)
        {
            _IsPlayersPool = true;
            _thisObjsCollider = transform.parent.GetComponent<Collider>();
        }
        else
        {
            GoonBase goon = transform.parent.GetComponent<GoonBase>();
            if (goon)
            {
                _IsPlayersPool = false;
                _thisObjsCollider = goon.GetComponent<Collider>();
            }
            else
            {
                Debug.LogError("Bullet pool must be on an instance of either the player or a goon.");
            }
        }
    }

    void Start()
    {
        // Create all the bullets in the pool
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject newBullet = Instantiate(_bulletPrefab);
            newBullet.GetComponent<Bullet>().SetIsPlayersBullet(_IsPlayersPool);
            newBullet.GetComponent<Bullet>().SetThisObjectsCollider(_thisObjsCollider);
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
                bullet.GetComponent<Bullet>().SetThisObjectsCollider(_thisObjsCollider);
                bullet.SetActive(true);
                return bullet;
            }
        }
        // If all are active
        return null;
    }
}
