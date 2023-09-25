using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region public variables
    public static PlayerController Instance;
    public bool IsDead { get; set; } = false;
    #endregion

    #region serialized variables
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] UnityEvent onMove = null;

    [Header("Chomp")]
    [SerializeField] Collider chompCollider = null;
    [SerializeField] float chompCooldown = 2f;
    //[SerializeField] string chompAxis = "";
    [SerializeField] UnityEvent OnChomp = null;

    [Header("Shoot Settings")]
    [SerializeField] BulletPool bulletPool = null;
    [SerializeField] float bulletVelocity = 15f;
    [SerializeField] float bulletLifeTime = 6f;
    [SerializeField] float bulletScaleAmount = 0.98f;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Animations")]
    [SerializeField] Animator animator = null;
    #endregion

    #region private variables
    ICombatAbility _combatAbility;
    CharacterController _controller;
    Camera _mainCamera;
    float _turnVelocity;
    bool _chompIsCoolingDown = false;
    bool _isDashing = false;
    bool _dashIsCoolingDown = false;
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
        if (chompCollider != null)
        {
            chompCollider.enabled = false;
        }
        else
        {
            Debug.LogWarning("The player controller must be given a collider.");
        }

        _controller = GetComponent<CharacterController>();
        _mainCamera = CameraMovement.Instance.gameObject.GetComponent<Camera>();
        // Instantiating combat ability
        //_combatAbility = new NoCombatAbility();
        //_combatAbility = new DashCombatAbility();
        _combatAbility = new ShootCombatAbility(bulletPool, bulletVelocity, bulletLifeTime, bulletScaleAmount);
    }

    private void Update()
    {
        if (!_isDashing)
        {
            Move();

            // If using goon-given ability
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DashCombatAbility dashAbility = _combatAbility as DashCombatAbility;
                if (dashAbility != null && !_dashIsCoolingDown)
                {
                    Dash();
                }
                else
                {
                    _combatAbility.UseAbility();
                }
            
            }
        }
        
    }

    public void SetCombatAbility(ICombatAbility combatAbility)
    {
        _combatAbility = combatAbility;
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        transform.forward = GetMouseDirection();

        if (direction.magnitude >= 0.1f)
        {
            // Moving the character
            _controller.Move(transform.forward * moveSpeed * Time.deltaTime);

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

    private void Dash()
    {
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;
        Vector3 dashDirection = transform.forward;
        _dashIsCoolingDown = true;
        _isDashing = true;

        // Dashing
        while (Time.time < startTime + dashDuration)
        {
            _controller.Move(dashDirection * dashSpeed * Time.deltaTime);

            yield return null;
        }
        _isDashing = false;

        // Dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        _dashIsCoolingDown = false;
    }

    /*

    [SerializeField]
    new Collider collider = null; //TODO
    
    private bool AttemptChomp()
    {
        if (!Input.GetButtonDown(chompAxis))
        {
            return false;
        }

        return Chomp();
    }

    private bool Chomp()
    {
        punchCollider.enabled = true;

        OnPunch?.Invoke();
        return true;
    }

    void PunchCheck()
    {
        if (Physics.CheckBox(collider.bounds.center, collider.bounds.extents / 2, Quaternion.identity, ~LayerMask.GetMask("Player")))
        {
            RaycastHit hitinfo;
            if (Physics.Raycast(collider.bounds.center, transform.forward, out hitinfo, 2, ~LayerMask.GetMask("Player")))
            {
                HitCollider(hitinfo.collider);
            }
        }
    }

    void OnTriggerStay(Collider other) => HitCollider(other);

    void OnTriggerEnter(Collider other) => HitCollider(other);

    void OnTriggerExit(Collider other) => HitCollider(other);

    void HitCollider(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
        {
            Enemy.EnemyBase enemy = other.transform.parent.GetComponent<Enemy.EnemyBase>();

            if (enemy != null)
            {
                enemy.Kill();
            }
            else
            {
                Debug.LogError("Unable to find enemy component to kill 'em");
            }
        }
    }
    */
}
