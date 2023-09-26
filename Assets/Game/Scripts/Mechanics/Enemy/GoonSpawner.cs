using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonSpawner : MonoBehaviour
{   
    public List<GameObject> InactiveGoons;
    public List<GameObject> ActiveGoons;

    [SerializeField] List<GameObject> goonObjectList;
    [SerializeField] float _spawnTime = 1f;
    [SerializeField] float _spawnDelay = 3f;

    private void Awake()
    {
        InactiveGoons = new List<GameObject>();
        ActiveGoons = new List<GameObject>();
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
            InactiveGoons.Add(newGoon);
        }

        // Repeatedly spawn goons
        InvokeRepeating(nameof(GetPooledObject), _spawnTime, _spawnDelay);
    }

    public GameObject GetPooledObject()
    {
        // Get random number from the inactive goon indices to choose a goon to spawn
        if (InactiveGoons.Count != 0)
        {
            int randomIndex = Random.Range(0, InactiveGoons.Count);
            GameObject goon = InactiveGoons[randomIndex];

            if (!goon.activeSelf)
            {
                goon.transform.position = transform.position;
                goon.transform.rotation = transform.rotation;
                goon.GetComponent<GoonBase>().Activate();
                return goon;
            }
            else
            {
                Debug.LogError("The inactive goon list should never give an active goon.");
                return null;
            }
        }
        else
        {
            Debug.Log("No goons left in spwaner.");
            return null;
        }
        
    }
}
