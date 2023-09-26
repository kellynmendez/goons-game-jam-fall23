using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> goonObjectList;
    [SerializeField] float _spawnTime = 1f;
    [SerializeField] float _spawnDelay = 3f;

    List<GameObject> _goonPool;
    List<int> _inactiveGoonIndices;
    private int _poolSize;

    private void Awake()
    {
        _goonPool = new List<GameObject>();
        _poolSize = goonObjectList.Count;
    }

    private void Start()
    {
        for (int i = 0; i < goonObjectList.Count; i++)
        {
            _inactiveGoonIndices.Add(i);
            // Instantiate goon and set it to inactive
            GameObject newGoon = Instantiate(goonObjectList[i]);
            newGoon.transform.SetPositionAndRotation(transform.position, transform.rotation);
            newGoon.SetActive(false);
            _goonPool.Add(newGoon);
        }

        // Repeatedly spawn goons
        InvokeRepeating(nameof(GetPooledObject), _spawnTime, _spawnDelay);
    }

    public GameObject GetPooledObject()
    {
        // Get random number from the inactive goon indices to choose a goon to spawn
        if (_inactiveGoonIndices.Count != 0)
        {
            int randomIndex = Random.Range(0, _poolSize);
            GameObject goon = _goonPool[_inactiveGoonIndices[randomIndex]];

            if (!goon.activeSelf)
            {
                goon.transform.position = transform.position;
                goon.transform.rotation = transform.rotation;
                goon.SetActive(true);
                return goon;
            }
            else
            {
                Debug.LogError("The inactive goon indices list should never give an active goon.");
                return null;
            }
        }
        else
            return null;
    }
}
