using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class GoonBase : MonoBehaviour
{
    public bool IsDead { get; set; } = false;

    [SerializeField] GameObject _visualsToDeactivate;
    [SerializeField] protected UnityEvent OnCombatAbility = null;
    //[SerializeField] protected UnityEvent OnHurt = null;
    [SerializeField] protected UnityEvent OnDeath = null;

    protected NavMeshAgent agent;
    protected GoonSpawner spawner;
    protected HealthSystem health;
    protected AudioSource audioSource;
    protected Collider _colliderToDeactivate;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = gameObject.GetComponent<HealthSystem>();
        audioSource = gameObject.GetComponent<AudioSource>();
        _colliderToDeactivate = gameObject.GetComponent<Collider>();

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
        OnDeath?.Invoke();

        // Add goon to inactive goon spawn list and remove from active list
        spawner.RemoveFromActiveGoonsList(this.gameObject);
        spawner.AddToInactiveGoonsList(this.gameObject);

        // Remove goon from killable goons lists (if in it)
        PlayerController.Instance.RemoveFromKillableGoonsList(this);

        // Disabling goon
        this.enabled = false;
        _visualsToDeactivate.SetActive(false);
        _colliderToDeactivate.enabled = false;
        IsDead = true;

        // Deactivating game object after sound
        StartCoroutine(DeactivateAfterSound());
    }

    public void Spawn()
    {
        // Enable goon
        _visualsToDeactivate.SetActive(true);
        _colliderToDeactivate.enabled = true;
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
        while (true)
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

    private IEnumerator DeactivateAfterSound()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        // Deactivating game object
        gameObject.SetActive(false);
    }
}
