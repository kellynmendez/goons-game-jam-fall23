using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    #region public variables
    public static PlayerController Instance;
    public bool IsDead { get; set; } = false;
    public bool IsUsingGoonAbility { get; set; } = false;
    public bool IsChomping { get; set; } = false;
    #endregion

    #region serialized variables
    [SerializeField] UnityEvent OnDeath = null;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;

    [Header("Chomp Settings")]
    //[SerializeField] float chompCooldown = 2f;
    [SerializeField] UnityEvent OnChomp = null;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] int numDashesAllowed = 5;

    [Header("Shoot Settings")]
    [SerializeField] BulletPool bulletPool = null;
    [SerializeField] float bulletVelocity = 15f;
    [SerializeField] float bulletLifeTime = 6f;
    [SerializeField] int numShotsAllowed = 12;

    [Header("Shield Settings")]
    [SerializeField] float invincibilityDuration = 8f;

    [Header("Hammer Settings")]
    [SerializeField] int numHammersAllowed = 3;

    [Header("PuttPutt Settings")]
    [SerializeField] ParticleSystem _puttPuttRingEffect;
    [SerializeField] int numPuttsAllowed = 1;

    //[Header("Animations")]
    //[SerializeField] Animator animator = null;
    #endregion

    #region private variables
    private ICombatAbility _combatAbility;
    private CharacterController _controller;
    private Camera _mainCamera;
    private List<GoonBase> _killableGoons;

    // Tracking 
    private int numShots = 0;
    private int numDashes = 0;
    private int numHammers = 0;
    private int numPutts = 0;
    //private bool _chompIsCoolingDown = false;

    // Inventory
    private ICombatAbility[] _goonAbilities;

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

        // Instantiating goon abilities
        _goonAbilities = new ICombatAbility[3];
        for (int i = 0; i < _goonAbilities.Length; i++)
        {
            _goonAbilities[i] = new NoCombatAbility();
        }

        _controller = GetComponent<CharacterController>();
        _killableGoons = new List<GoonBase>();
        _audioSource = GetComponent<AudioSource>();

        // Instantiating combat ability
        _combatAbility = new NoCombatAbility();
    }

    private void Start()
    {
        _mainCamera = CameraMovement.Instance.gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if (IsUsingGoonAbility)
            return;

        Move();

        // If using goon-given ability
        if (Input.GetMouseButtonDown(0))
        {
            _combatAbility.UseAbility();
            CheckIfCombatAbilityIsUsedUp();
        }
        // If chomping
        else if (!IsChomping && Input.GetMouseButtonDown(1))
        {
            Chomp();
        }
    }

    public void SetCombatAbility(ICombatAbility combatAbility)
    {
        _combatAbility = combatAbility;
    }

    public void CheckIfCombatAbilityIsUsedUp()
    {
        if (_combatAbility is ShootCombatAbility)
        {
            numShots++;
            if (numShots >= numShotsAllowed)
            {
                InventorySystem.Instance.RemoveGoonAbilityFromInventory();
                numShots = 0;
            }
        }
        else if (_combatAbility is DashCombatAbility)
        {
            numDashes++;
            if (numDashes >= numDashesAllowed)
            {
                InventorySystem.Instance.RemoveGoonAbilityFromInventory();
                numDashes = 0;
            }
        }
        else if (_combatAbility is HammerCombatAbility)
        {
            numHammers++;
            if (numHammers >= numHammersAllowed)
            {
                InventorySystem.Instance.RemoveGoonAbilityFromInventory();
                numHammers = 0;
            }
        }
        else if (_combatAbility is PuttPuttCombatAbility)
        {
            numPutts++;
            if (numPutts >= numPuttsAllowed)
            {
                InventorySystem.Instance.RemoveGoonAbilityFromInventory();
                numPutts = 0;
            }
        }
    }

    #region Movement functions
    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        transform.forward = GetMouseDirection();

        if (direction.magnitude >= 0.1f)
        {
            // Moving the character
            _controller.Move(direction * moveSpeed * Time.deltaTime);
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
        IsChomping = true;
        
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
            InventorySystem.Instance.AddGoonAbilityToInventory(goonToChomp);
            goonToChomp.Kill();
            // Invoke chomp event
            OnChomp.Invoke();
        }
            
        IsChomping = false;
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
        return new ShieldCombatAbility(this, gameObject.GetComponent<HealthSystem>(), invincibilityDuration);
    }

    public DashCombatAbility GetPlayerSpeedCombatAbility()
    {
        return new DashCombatAbility(this, _controller, dashSpeed, dashDuration, dashCooldown);
    }

    public HammerCombatAbility GetPlayerHammerCombatAbility()
    {
        return new HammerCombatAbility(this, _killableGoons);
    }

    public PuttPuttCombatAbility GetPlayerPuttPuttCombatAbility()
    {
        return new PuttPuttCombatAbility(this, _puttPuttRingEffect);
    }
    #endregion


    /// <summary>
    /// Freeze player rotation and position when using abilities
    /// </summary>
    public void FreezePlayerTransform()
    {

    }
    
    public void Kill()
    {
        OnDeath?.Invoke();
        IsDead = true;
        this.enabled = false;
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
}
