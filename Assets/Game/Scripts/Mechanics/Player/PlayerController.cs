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
    public bool IsDashing { get; set; } = false;
    #endregion

    #region serialized variables
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] UnityEvent onMove = null;

    [Header("Chomp")]
    [SerializeField] Collider chompCollider = null;
    [SerializeField] LayerMask enemyLayerMask;
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

    [Header("Shield Settings")]
    [SerializeField] float invincibilityDuration = 5f;

    [Header("Animations")]
    [SerializeField] Animator animator = null;
    #endregion

    #region private variables
    private ICombatAbility _combatAbility;
    private CharacterController _controller;
    private Camera _mainCamera;
    private List<GoonBase> _chompList;
    private bool _chompIsCoolingDown = false;
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
        _chompList = new List<GoonBase>();

        // Instantiating combat ability
        //_combatAbility = new NoCombatAbility();
        //_combatAbility = new DashCombatAbility(this, _controller, dashSpeed, dashDuration, dashCooldown);
        _combatAbility = new ShootCombatAbility(bulletPool, bulletVelocity, bulletLifeTime, bulletScaleAmount);
        //_combatAbility = new ShieldCombatAbility(this, gameObject.GetComponent<HealthSystem>(), invincibilityDuration);
    }

    private void Update()
    {
        if (IsDashing)
            return;

        Move();

        // If using goon-given ability
        if (Input.GetMouseButtonDown(0))
        {
            _combatAbility.UseAbility();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            
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

    private void Chomp()
    {
        /*
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            Debug.Log("Hit : " + hitColliders[i].name + i);
            //Increase the number of Colliders in the array
            i++;
        }*/
    }

    public void AddGoonToChompList(GoonBase goon)
    {
        _chompList.Add(goon);
    }

    public void RemoveGoonFromChompList(GoonBase goon)
    {
        _chompList.Remove(goon);
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
