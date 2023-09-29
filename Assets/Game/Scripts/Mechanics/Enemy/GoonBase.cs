using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class GoonBase : MonoBehaviour
{
    public bool IsDead { get; set; } = false;

    [SerializeField] protected UnityEvent OnCombatAbility = null;

    protected NavMeshAgent agent;
    protected GoonSpawner spawner;
    protected HealthSystem health;
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = gameObject.GetComponent<HealthSystem>();
        audioSource = gameObject.GetComponent<AudioSource>();

        //StartCoroutine(ChasePlayer());
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

        // Disabling goon
        this.enabled = false;
        IsDead = true;

        // Add goon to inactive goon spawn list and remove from active list
        spawner.RemoveFromActiveGoonsList(this.gameObject);
        spawner.AddToInactiveGoonsList(this.gameObject);

        // Remove goon from killable goons lists (if in it)
        PlayerController.Instance.RemoveFromKillableGoonsList(this);
        
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
        spawner.RemoveFromInactiveGoonsList(this.gameObject);
        spawner.AddToActiveGoonsList(this.gameObject);
    }

    public void SetSpawner(GoonSpawner spawner)
    {
        this.spawner = spawner;
    }

    private IEnumerator ChasePlayer()
    {
        while(true)
        {
            Debug.Log("chasing");
            agent.SetDestination(PlayerController.Instance.transform.position);
            //yield return new WaitForSeconds(0.1f);
            yield return null;
        }
    }
    public void PlayFX(AudioClip sfx)
    {
        if (audioSource != null && sfx != null)
        {
            audioSource.PlayOneShot(sfx, audioSource.volume);
        }
        else
        {
            Debug.LogError("Must have audio component and give sfx.");
        }
    }
}
