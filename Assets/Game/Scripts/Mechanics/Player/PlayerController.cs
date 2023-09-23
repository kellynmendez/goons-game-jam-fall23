using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region public variables
    public int Health { get; private set; }
    public bool IsDead { get; private set; } = false;
    public Vector3 Velocity { get; set; } = Vector3.zero;
    #endregion

    #region serialized variables
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float turnTime = 0.1f;
    [SerializeField] UnityEvent OnMove = null;

    [Header("Health")]
    [SerializeField] int lives = 3;
    [SerializeField] UnityEvent OnHurt = null;
    [SerializeField] UnityEvent OnDeath = null;

    [Header("Chomp")]
    [SerializeField] Collider chompCollider = null;
    [SerializeField] float chompCooldown = 2f;
    //[SerializeField] string chompAxis = "";
    [SerializeField] UnityEvent OnChomp = null;

    [Header("Animations")]
    [SerializeField] Animator animator = null;
    #endregion

    #region private variables
    CharacterController _controller;
    Vector3 _inputMove = Vector3.zero;
    float _turnVelocity;
    bool _chompIsCoolingDown = false;
    #endregion

    private void Awake()
    {
        // Collider check
        if (chompCollider != null)
        {
            chompCollider.enabled = false;
        }

        _controller = GetComponent<CharacterController>();
        Health = lives;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Turning character in the direction it moving
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Moving the character
            _controller.Move(direction * moveSpeed * Time.deltaTime);
        }
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

    /*
    public void Damage(int amt, Vector3 from)
    {
        if (IsDead) return;
        if (amt < 0) throw new System.ArgumentOutOfRangeException($"amt must be a non-negative integer. {amt} was provided.");
        Health -= amt;

        OnHurt.Invoke();

        if (Health <= 0)
        {
            Kill(from);
        }
    }
    */

    /*
    public void Kill(Vector3 from)
    {
        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.useGravity = true;

        // apply death force
        _rigidbody.AddForce((transform.position - from).normalized * 500 + (Vector3.up * 200));
        _rigidbody.AddTorque(transform.right * 500);

        this.enabled = false;
        IsDead = true;

        OnDeath.Invoke();

        Health = 0;
    }
    */
}
