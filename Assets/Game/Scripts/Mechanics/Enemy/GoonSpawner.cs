using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonSpawner : MonoBehaviour
{   
    private List<GameObject> _inactiveGoons;
    private List<GameObject> _activeGoons;

    [SerializeField] List<GameObject> goonObjectList;
    [SerializeField] float _spawnTime = 1f;
    [SerializeField] float _spawnDelay = 3f;

    private void Awake()
    {
        _inactiveGoons = new List<GameObject>();
        _activeGoons = new List<GameObject>();
    }

    private void Start()
    {
        for (int i = 0; i < goonObjectList.Count; i++)
        {
            // Instantiate goon and set it to inactive
            GameObject newGoon = Instantiate(goonObjectList[i]);
            newGoon.GetComponent<GoonBase>().SetSpawner(this);
            newGoon.transform.SetPositionAndRotation(transform.position, transform.rotation);
            newGoon.SetActive(false);
            _inactiveGoons.Add(newGoon);
        }

        // Repeatedly spawn goons
        InvokeRepeating(nameof(GetPooledObject), _spawnTime, _spawnDelay);
    }

    public GameObject GetPooledObject()
    {
        // Get random number from the inactive goons to choose a goon to spawn
        if (_inactiveGoons.Count != 0)
        {
            int randomIndex = Random.Range(0, _inactiveGoons.Count);
            GoonBase goon = _inactiveGoons[randomIndex].GetComponent<GoonBase>();

            if (!goon.gameObject.activeSelf)
            {
                goon.gameObject.SetActive(true);
                goon.gameObject.transform.position = transform.position;
                goon.gameObject.transform.rotation = transform.rotation;
                goon.Spawn();
                return goon.gameObject;
            }
            else
            {
                Debug.LogError("The inactive goon list should never give an active goon.");
                return null;
            }
        }
        else
        {
            Debug.Log("No goons left in spawner.");
            return null;
        }
        
    }

    public void AddToInactiveGoonsList(GameObject goon)
    {
        _inactiveGoons.Add(goon);
    }

    public void RemoveFromInactiveGoonsList(GameObject goon)
    {
        _inactiveGoons.Remove(goon);
    }

    public void AddToActiveGoonsList(GameObject goon)
    {
        _activeGoons.Add(goon);
    }

    public void RemoveFromActiveGoonsList(GameObject goon)
    {
        _activeGoons.Remove(goon);
    }
}
