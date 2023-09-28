using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GoonBase : MonoBehaviour
{
    public bool IsDead { get; set; } = false;

    [SerializeField] UnityEvent OnDeath = null;

    protected NavMeshAgent agent;
    protected GoonSpawner spawner;
    protected HealthSystem health;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = gameObject.GetComponent<HealthSystem>();
    }

    protected virtual void Update()
    {
        agent.SetDestination(PlayerController.Instance.transform.position);

        if (PlayerController.Instance.IsDead)
        {
            this.enabled = false;
        }
    }

    public void Kill()
    {
        if (IsDead) return;
        Debug.Log($"{gameObject.name} killed!");
        // Disabling goon
        this.enabled = false;
        IsDead = true;

        // Invoke death event
        OnDeath?.Invoke();

        // Add goon to inactive goon spawn list and remove from active list
        spawner.ActiveGoons.Remove(this.gameObject);
        spawner.InactiveGoons.Add(this.gameObject);
        
        // Deactivating game object
        gameObject.SetActive(false);
    }

    public void Spawn()
    {
        // Enable goon
        this.enabled = true;
        IsDead = false;
        
        // Reset its lives
        health.ResetLives();
        health.IsDead = false;

        // Add goon to active list and remove from inactive list
        spawner.InactiveGoons.Remove(this.gameObject);
        spawner.ActiveGoons.Add(this.gameObject);
        Debug.Log($"{gameObject.name} activated, lives are {health.LivesLeft}");
    }

    public void SetSpawner(GoonSpawner spawner)
    {
        this.spawner = spawner;
    }

}
