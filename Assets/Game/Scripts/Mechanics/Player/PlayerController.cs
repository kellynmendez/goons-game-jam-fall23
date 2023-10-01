using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    #region public variables
    public static PlayerController Instance;
    public bool IsMoving { get; set; } = false;
    public bool IsDead { get; set; } = false;
    public bool IsInvincible { get; set; } = false;
    public bool IsUsingGoonAbility { get; set; } = false;
    public bool IsChomping { get; set; } = false;
    public bool IsFrozen { get; set; } = false;
    #endregion

    #region serialized variables
    [Header("Health")]
    [SerializeField] int lives = 3;
    [SerializeField] GameObject visualsToDeactivate;
    [SerializeField] UnityEvent OnHurt = null;
    [SerializeField] UnityEvent OnDeath = null;
    [SerializeField] float knockbackDuration = 0.6f;
    [SerializeField] float knockbackForce = 4f;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] Animator animator;

    [Header("Chomp Settings")]
    //[SerializeField] float chompCooldown = 2f;
    [SerializeField] UnityEvent OnChomp = null;
    [SerializeField] float chompCooldown = 3f;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] public int numDashesAllowed = 5;
    [SerializeField] ParticleSystem dashFX;

    [Header("Shoot Settings")]
    [SerializeField] BulletPool bulletPool = null;
    [SerializeField] float bulletVelocity = 15f;
    [SerializeField] float bulletLifeTime = 6f;
    [SerializeField] public int numShotsAllowed = 12;

    [Header("Shield Settings")]
    [SerializeField] float invincibilityDuration = 8f;
    [SerializeField] ParticleSystem shieldInvincibilityVFX;

    [Header("Hammer Settings")]
    [SerializeField] public int numHammersAllowed = 3;

    [Header("PuttPutt Settings")]
    [SerializeField] ParticleSystem puttPuttRingVFX;
    [SerializeField] public int numPuttsAllowed = 1;

    [Header("Audio")]
    [SerializeField] AudioClip noAbilityClip = null;
    [SerializeField] AudioClip shootAbilityClip = null;
    [SerializeField] AudioClip shieldAbilityClip = null;
    [SerializeField] AudioClip speedAbilityClip = null;
    [SerializeField] AudioClip hammerAbilityClip = null;
    [SerializeField] AudioClip puttAbilityClip = null;
    #endregion

    #region private variables
    private int _livesLeft;
    private ICombatAbility _combatAbility;
    private CharacterController _controller;
    private Camera _mainCamera;
    private List<GoonBase> _killableGoons;

    // Tracking goon ability uses
    private int _numShots;
    private int _numDashes;
    private int _numHammers;
    private int _numPutts;
    private bool _chompIsCoolingDown = false;

    // Animations
    protected const string IDLE_ANIM = "Idle";
    protected const string WALK_ANIM = "Walk";
    protected const string CHOMP_ANIM = "Chomp";
    protected const string HAMMER_ANIM = "Hammer";

    // FX
    private AudioSource _audioSource;
    #endregion

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There should not be more than one player in a scene.");
        }

        // Bullet origin check
        if (bulletPool == null)
        {
            Debug.LogError("There is no bullet pool on this object or its children.");
        }

        // Filling in references
        _controller = GetComponent<CharacterController>();
        _killableGoons = new List<GoonBase>();
        _audioSource = GetComponent<AudioSource>();
        _livesLeft = lives;

        // Instantiating combat ability
        _combatAbility = new NoCombatAbility();

        // Setting start ability values
        _numShots = numShotsAllowed;
        _numDashes = numDashesAllowed;
        _numHammers = numHammersAllowed;
        _numPutts = numPuttsAllowed;
    }

    private void Start()
    {
        // Filling in main camera reference
        _mainCamera = CameraMovement.Instance.gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if (IsUsingGoonAbility)
            return;

        if (!IsFrozen)
        {
            Move();
        }

        // If using goon-given ability
        if (Input.GetMouseButtonDown(0))
        {
            if (_combatAbility is NoCombatAbility)
            {
                PlayFX(noAbilityClip);
            }
            else
            {
                _combatAbility.UseAbility();
                CheckIfCombatAbilityIsUsedUp();
            }
        }
        // If chomping
        else if (!_chompIsCoolingDown && Input.GetMouseButtonDown(1))
        {
            Chomp();
        }
    }

    #region Combat ability switches
    public void SetCombatAbility(ICombatAbility combatAbility)
    {
        _combatAbility = combatAbility;
    }

    /// <summary>
    /// Checks if the ability the player has current has been used up; if it has
    /// then remove it from the player's inventory
    /// </summary>
    public void CheckIfCombatAbilityIsUsedUp()
    {
        if (_combatAbility is ShootCombatAbility)
        {
            _numShots--;
            UIManager.Instance.UpdateCurrentAbilityText(_numShots);
            PlayFX(shootAbilityClip);

            if (_numShots <= 0)
            {
                UIManager.Instance.RemoveGoonAbilityFromInventory();
                _numShots = numShotsAllowed;
            }
        }
        else if (_combatAbility is ShieldCombatAbility)
        {
            UIManager.Instance.UpdateCurrentAbilityText(0);
            UIManager.Instance.RemoveGoonAbilityFromInventory();
            PlayFX(shieldAbilityClip);
        }
        else if (_combatAbility is DashCombatAbility)
        {
            _numDashes--;
            UIManager.Instance.UpdateCurrentAbilityText(_numDashes);
            PlayFX(speedAbilityClip);

            if (_numDashes <= 0)
            {
                UIManager.Instance.RemoveGoonAbilityFromInventory();
                _numDashes = numDashesAllowed;
            }
        }
        else if (_combatAbility is HammerCombatAbility)
        {
            _numHammers--;
            UIManager.Instance.UpdateCurrentAbilityText(_numHammers);
            PlayFX(hammerAbilityClip);
            animator.Play(HAMMER_ANIM);

            if (_numHammers <= 0)
            {
                UIManager.Instance.RemoveGoonAbilityFromInventory();
                _numHammers = numHammersAllowed;
            }
        }
        else if (_combatAbility is PuttPuttCombatAbility)
        {
            _numPutts--;
            UIManager.Instance.UpdateCurrentAbilityText(_numPutts);
            PlayFX(puttAbilityClip);

            if (_numPutts <= 0)
            {
                UIManager.Instance.RemoveGoonAbilityFromInventory();
                _numPutts = numPuttsAllowed;
            }
        }
    }
    #endregion

    #region Movement functions
    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        transform.forward = GetMouseDirection();

        if (direction.magnitude >= 0.1f)
        {
            // Playing move animation
            if (!IsMoving)
            {
                IsMoving = true;
                animator.Play(WALK_ANIM);
            }
            // Moving the character
            _controller.Move(direction * moveSpeed * Time.deltaTime);
            
        }
        else
        {
            // Playing idle animation
            if (IsMoving && !IsChomping)
            {
                IsMoving = false;
                animator.Play(IDLE_ANIM);
            }
        }
    }

    private Vector3 GetMouseDirection()
    {
        Vector3 input = Input.mousePosition;
        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(new Vector3(input.x, input.y, _mainCamera.transform.position.y));
        Vector3 mouseDirection = (mousePosition - transform.position).normalized;
        mouseDirection.y = 0;
        return mouseDirection;
    }
    #endregion

    #region Chomp functions
    private void Chomp()
    {
        StartCoroutine(ChompCoroutine());
    }

    public void AddToKillableGoonsList(GoonBase goon)
    {
        _killableGoons.Add(goon);
    }

    public void RemoveFromKillableGoonsList(GoonBase goon)
    {
        _killableGoons.Remove(goon);
    }
    #endregion

    #region Get Combat Abilities
    public ShootCombatAbility GetPlayerShootCombatAbility()
    {
        return new ShootCombatAbility(bulletPool, bulletVelocity, bulletLifeTime);
    }

    public ShieldCombatAbility GetPlayerShieldCombatAbility()
    {
        return new ShieldCombatAbility(this, this, null, invincibilityDuration, shieldInvincibilityVFX);
    }

    public DashCombatAbility GetPlayerSpeedCombatAbility()
    {
        return new DashCombatAbility(this, _controller, dashSpeed, dashDuration, dashCooldown, dashFX);
    }

    public HammerCombatAbility GetPlayerHammerCombatAbility()
    {
        return new HammerCombatAbility(this, _killableGoons);
    }

    public PuttPuttCombatAbility GetPlayerPuttPuttCombatAbility()
    {
        return new PuttPuttCombatAbility(this, puttPuttRingVFX);
    }
    #endregion


    /// <summary>
    /// Freeze player rotation and position when using abilities
    /// </summary>
    private IEnumerator FreezePlayerTransform(float duration)
    {
        IsFrozen = true;
        transform.forward = GetMouseDirection();
        yield return new WaitForSeconds(duration);
        IsFrozen = false;
    }

    public void Hurt(Vector3 knockbackDir)
    {
        if (IsDead)
            return;

        if (IsInvincible)
            return;

        StartCoroutine(Knockback(knockbackDir));

        _livesLeft -= 1;

        OnHurt.Invoke();

        UIManager.Instance.PlayerHurt();

        if (_livesLeft <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        OnDeath?.Invoke();
        IsDead = true;
        this.enabled = false;
        visualsToDeactivate.SetActive(false);

        StartCoroutine(EndGame());
    }

    public void PlayFX(AudioClip sfx)
    {
        if (_audioSource != null && sfx != null)
        {
            _audioSource.PlayOneShot(sfx, _audioSource.volume);
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

    private IEnumerator Knockback(Vector3 knockbackDir)
    {
        float startTime = Time.time;
        Vector3 dashDirection = _controller.transform.forward;
        float tempTime = 0;
        float currKnockbackSpeed = knockbackForce;
        // Dashing
        while (Time.time < startTime + knockbackDuration)
        {
            tempTime += Time.deltaTime;
            if (tempTime > 0.1f)
            {
                tempTime = 0;
                currKnockbackSpeed *= 0.85f;
            }
            _controller.Move(knockbackDir * currKnockbackSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator ChompCoroutine()
    {
        IsChomping = true;
        IsFrozen = true;
        transform.forward = GetMouseDirection();

        float animWait = 0.3f;
        yield return new WaitForSeconds(animWait);

        OnChomp.Invoke();
        animator.Play(CHOMP_ANIM);

        if (_killableGoons.Count == 0)
        {
            Debug.Log("Nothing to chomp!");
        }
        else // Getting the goon to be chomped
        {
            GoonBase goonToChomp = null;
            if (_killableGoons.Count == 1)
            {
                goonToChomp = _killableGoons[0];
            }
            else
            {
                // Checking goon list to find which one is closer
                float smallestDistance = 0;
                for (int i = 0; i < _killableGoons.Count; i++)
                {
                    float distance = Vector3.Distance(transform.position, _killableGoons[i].transform.position);

                    if (goonToChomp == null)
                    {
                        smallestDistance = distance;
                        goonToChomp = _killableGoons[i];
                    }
                    else
                    {
                        // If this goon is closer, then chomp this one
                        if (distance < smallestDistance)
                        {
                            smallestDistance = distance;
                            goonToChomp = _killableGoons[i];
                        }
                    }
                }
            }

            // Chomp goon
            UIManager.Instance.AddGoonAbilityToInventory(goonToChomp);
            goonToChomp.Kill();
        }
        _chompIsCoolingDown = true;
        StartCoroutine(ChompCoolDown());
    }
    
    private IEnumerator ChompCoolDown()
    {
        float animWait = 0.7f;
        yield return new WaitForSeconds(animWait);
        IsChomping = false;
        IsFrozen = false;
        yield return new WaitForSeconds(chompCooldown - animWait);
        _chompIsCoolingDown = false;
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
