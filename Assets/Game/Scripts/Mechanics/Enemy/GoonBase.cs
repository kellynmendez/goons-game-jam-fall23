using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class GoonBase : MonoBehaviour
{
    public int EnemyCount = 0;
    public bool Dead { get; private set; } = false;

    [SerializeField] UnityEvent OnSpawn = null;
    [SerializeField] UnityEvent OnDeath = null;
    [SerializeField] new Collider collider = null;

    protected NavMeshAgent agent;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // OnAwake();
        // Spawn();
    }

    protected virtual void Update()
    {
        agent.SetDestination(PlayerController.Instance.transform.position);
    }

    protected virtual void OnAwake() { }

    protected virtual void Kill()
    {
        if (Dead) return;
        Dead = true;

        OnDeath?.Invoke();

        this.enabled = false;
        EnemyCount--;
    }

    public virtual void Spawn()
    {
        OnSpawn?.Invoke();
        EnemyCount++;
    }

}
