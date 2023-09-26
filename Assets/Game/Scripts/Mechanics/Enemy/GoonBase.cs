using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GoonBase : MonoBehaviour
{
    public int EnemyCount = 0;
    public bool IsDead { get; set; } = false;

    [SerializeField] UnityEvent OnSpawn = null;
    [SerializeField] UnityEvent OnDeath = null;
    [SerializeField] new Collider collider = null;

    protected NavMeshAgent agent;
    protected GoonSpawner spawner;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // OnAwake();
        // Spawn();
    }

    protected virtual void Update()
    {
        agent.SetDestination(PlayerController.Instance.transform.position);

        if (PlayerController.Instance.IsDead)
        {
            this.enabled = false;
        }
    }

    protected virtual void Kill()
    {
        if (IsDead) return;
        IsDead = true;

        OnDeath?.Invoke();

        this.enabled = false;
        EnemyCount--;
    }

    public void Activate()
    {
        IsDead = false;
        OnSpawn?.Invoke();
        spawner.InactiveGoons.Remove(this.gameObject);
        spawner.ActiveGoons.Add(this.gameObject);
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        IsDead = true;
        spawner.ActiveGoons.Remove(this.gameObject);
        spawner.InactiveGoons.Add(this.gameObject);
        gameObject.SetActive(false);
    }

    public void SetSpawner(GoonSpawner spawner)
    {
        this.spawner = spawner;
    }

}
