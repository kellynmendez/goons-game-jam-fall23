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
    public int LivesLeft { get; private set; }
    public bool IsDead { get; private set; } = false;
    #endregion

    #region serialized variables
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float turnTime = 0.1f;
    [SerializeField] UnityEvent onMove = null;

    [Header("Health")]
    [SerializeField] int lives = 3;
    [SerializeField] UnityEvent onHurt = null;
    [SerializeField] UnityEvent onDeath = null;

    [Header("Chomp")]
    [SerializeField] Collider chompCollider = null;
    [SerializeField] float chompCooldown = 2f;
    //[SerializeField] string chompAxis = "";
    [SerializeField] UnityEvent OnChomp = null;

    [Header("Shoot Ability")]
    [SerializeField] BulletPool bulletPool = null;
    [SerializeField] float bulletVelocity = 15f;
    [SerializeField] float bulletLifeTime = 6f;
    [SerializeField] float bulletScaleAmount = 0.98f;

    [Header("Animations")]
    [SerializeField] Animator animator = null;
    #endregion

    #region private variables
    ICombatAbility _combatAbility;
    CharacterController _controller;
    float _turnVelocity;
    bool _chompIsCoolingDown = false;
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

        // Instantiating combat ability
        _combatAbility = new NoCombatAbility();
        //_combatAbility = new ShootCombatAbility(bulletPool, bulletVelocity, bulletLifeTime, bulletScaleAmount);

        // Grabbing the controller and setting initial health
        _controller = GetComponent<CharacterController>();
        LivesLeft = lives;
    }

    private void Update()
    {
        Move();

        // If using goon-given ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _combatAbility.UseAbility();
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

        if (direction.magnitude >= 0.1f)
        {
            // Turning character in the direction it is moving
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Moving the character
            _controller.Move(direction * moveSpeed * Time.deltaTime);

            // Invoking on move unity event
            onMove.Invoke();
        }
    }

    public void Hurt(int damage)
    {
        if (IsDead)
            return;
        // Take damage
        LivesLeft -= damage;
        // Invoke on hurt unity event
        onHurt.Invoke();
        // Kill if that was the last life
        if (LivesLeft <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        // Disable player and set death bool
        this.enabled = false;
        IsDead = true;
        // Invoke on death unity event
        onDeath.Invoke();
        // Set lives to 0
        LivesLeft = 0;
    }

    /*
    [SerializeField] Transform chompOrigin = null;

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
