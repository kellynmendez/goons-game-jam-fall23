using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class GoonBase : MonoBehaviour
{
    public bool IsDead { get; set; } = false;
    public bool IsInvincible { get; set; } = false;

    [Header("Combat")]
    [SerializeField] protected UnityEvent OnCombatAbility = null;

    [Header("Health")]
    [SerializeField] int lives = 1;
    [SerializeField] GameObject _visualsToDeactivate;
    [SerializeField] protected UnityEvent OnHurt = null;
    [SerializeField] protected UnityEvent OnDeath = null;

    protected int livesLeft;
    protected NavMeshAgent agent;
    protected GoonSpawner spawner;
    protected HealthSystem health;
    protected AudioSource audioSource;
    protected Collider colliderToDeactivate;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = gameObject.GetComponent<HealthSystem>();
        audioSource = gameObject.GetComponent<AudioSource>();
        colliderToDeactivate = gameObject.GetComponent<Collider>();

        IsInvincible = false;
        livesLeft = lives;
        //StartCoroutine(ChasePlayer()); TODO
    }

    protected virtual void Update()
    {
        agent.SetDestination(PlayerController.Instance.transform.position);

        // TODO: make freezing enemies more efficient on death of player
        if (PlayerController.Instance.IsDead)
        {
            this.enabled = false;
        }
    }

    public void Spawn()
    {
        // Enable goon
        _visualsToDeactivate.SetActive(true);
        colliderToDeactivate.enabled = true;
        this.enabled = true;
        IsDead = false;
        
        // Reset its lives
        ResetLives();

        // Add goon to active list and remove from inactive list
        spawner.RemoveFromInactiveGoonsList(this.gameObject);
        spawner.AddToActiveGoonsList(this.gameObject);
    }

    public void Hurt()
    {
        if (IsDead)
            return;

        if (IsInvincible)
            return;

        livesLeft -= 1;

        if (livesLeft <= 0)
        {
            Kill();
        }
        else
        {
            OnHurt.Invoke();
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
        colliderToDeactivate.enabled = false;
        IsDead = true;

        // Deactivating game object after sound
        StartCoroutine(DeactivateAfterSound());
    }

    public void ResetLives()
    {
        livesLeft = lives;
    }

    public void SetSpawner(GoonSpawner spawner)
    {
        this.spawner = spawner;
    }

    // TODO: nav mesh set destination efficiency fix
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
