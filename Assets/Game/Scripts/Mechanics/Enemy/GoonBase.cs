using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class GoonBase : MonoBehaviour
{
    public bool IsDead { get; set; } = false;
    public bool IsMoving { get; set; } = true;

    [Header("Combat")]
    [SerializeField] protected UnityEvent OnCombatAbility = null;

    [Header("Health")]
    [SerializeField] int lives = 1;
    [SerializeField] GameObject _visualsToDeactivate;
    [SerializeField] protected UnityEvent OnHurt = null;
    [SerializeField] protected UnityEvent OnDeath = null;

    [Header("Animations")]
    [SerializeField] protected Animator _animator;

    protected const string IDLE_ANIM = "Idle";
    protected const string WALK_ANIM = "Walk";
    protected const string ATTACK_ANIM = "Attack";

    protected int livesLeft;
    protected NavMeshAgent agent;
    protected GoonSpawner spawner;
    protected AudioSource audioSource;
    protected Collider colliderToDeactivate;

    private float _tempTime = 0;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = gameObject.GetComponent<AudioSource>();
        colliderToDeactivate = gameObject.GetComponent<Collider>();

        livesLeft = lives;
    }

    protected virtual void Update()
    {
        // Setting destination every tenth of a second
        _tempTime += Time.deltaTime;
        if (_tempTime > 0.1f)
        {
            _tempTime = 0;
            agent.SetDestination(PlayerController.Instance.transform.position);
        }

        transform.LookAt(PlayerController.Instance.transform.position);

        // Checking if agent has stopped
        if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance) && 
            (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
            agent.isStopped = true;
            if (IsMoving == true)
            {
                IsMoving = false;
                _animator.Play(IDLE_ANIM);
            }
        }
        else
        {
            agent.isStopped = false;
            if (IsMoving == false)
            {
                IsMoving = true;
                _animator.Play(WALK_ANIM);
                
            }
        }

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

    public virtual void Hurt()
    {
        if (IsDead)
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
        // Add goon to inactive goon spawn list and remove from active list
        spawner.RemoveFromActiveGoonsList(this.gameObject);
        spawner.AddToInactiveGoonsList(this.gameObject);
        
        if (IsDead) return;

        OnDeath?.Invoke();

        // Remove goon from killable goons lists (if in it)
        PlayerController.Instance.RemoveFromKillableGoonsList(this);
        PlayerController.Instance.AddToKillableGoonsRingList(this);

        // Update score
        UIManager.Instance.IncrementScore();

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

    public void PlayVFX(ParticleSystem ps)
    {
        ps?.Play();
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
