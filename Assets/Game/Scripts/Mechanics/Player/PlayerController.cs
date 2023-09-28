using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region public variables
    public static PlayerController Instance;
    public bool IsDead { get; set; } = false;
    public bool IsUsingGoonAbility { get; set; } = false;
    public bool IsChomping { get; set; } = false;
    #endregion

    #region serialized variables
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] UnityEvent onMove = null;

    [Header("Chomp")]
    [SerializeField] Collider chompCollider = null;
    //[SerializeField] float chompCooldown = 2f;
    //[SerializeField] UnityEvent OnChomp = null;

    [Header("Shoot Settings")]
    [SerializeField] BulletPool bulletPool = null;
    [SerializeField] float bulletVelocity = 15f;
    [SerializeField] float bulletLifeTime = 6f;
    [SerializeField] float bulletScaleAmount = 0.98f;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Shield Settings")]
    [SerializeField] float invincibilityDuration = 5f;

    //[Header("Animations")]
    //[SerializeField] Animator animator = null;
    #endregion

    #region private variables
    private ICombatAbility _combatAbility;
    private CharacterController _controller;
    private Camera _mainCamera;
    private List<GoonBase> _killableGoons;
    //private bool _chompIsCoolingDown = false;
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
            Debug.LogError("There should not be more than two players in a secene.");
        }

        // Collider check
        if (chompCollider == null)
        {
            Debug.LogError("The player controller must be given a collider.");
        }

        _controller = GetComponent<CharacterController>();
        _mainCamera = CameraMovement.Instance.gameObject.GetComponent<Camera>();
        _killableGoons = new List<GoonBase>();

        // Instantiating combat ability
        //_combatAbility = new NoCombatAbility();
        //_combatAbility = new DashCombatAbility(this, _controller, dashSpeed, dashDuration, dashCooldown);
        //_combatAbility = new ShootCombatAbility(bulletPool, bulletVelocity, bulletLifeTime, bulletScaleAmount);
        //_combatAbility = new ShieldCombatAbility(this, gameObject.GetComponent<HealthSystem>(), invincibilityDuration);
        _combatAbility = new HammerCombatAbility(this, _killableGoons);
        //_combatAbility = new PuttPuttCombatAbility();
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

            // Invoking on move unity event
            onMove.Invoke();
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
            goonToChomp.Kill();
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

    public void FreezePlayerTransform()
    {

    }
    
    public void Kill()
    {
        IsDead = true;
        this.enabled = false;
    }
}
