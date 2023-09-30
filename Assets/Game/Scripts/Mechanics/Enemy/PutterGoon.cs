using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutterGoon : GoonBase
{
    [Header("Putt Putt Settings")]
    [SerializeField] float _attackInterval = 2.5f;
    [SerializeField] float _pauseBeforeAttack = 0.5f;
    [SerializeField] Collider _hitCollider;
    [SerializeField] ParticleSystem puttAttackFX;

    private HitCombatAbility _hitAbility;

    protected override void Awake()
    {
        base.Awake();

        if (_hitCollider == null)
        {
            Debug.LogError("There is no hit collider on this object or its children.");
        }
        else
        {
            _hitCollider.enabled = false;
        }
    }

    private void Start()
    {
        // Establishing the enemy's ability
        _hitAbility = new HitCombatAbility(this, _hitCollider);

        StartCoroutine(Hit());
    }

    private IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        yield return new WaitForSeconds(_pauseBeforeAttack);
                        // Using ability and invoking unity event
                        puttAttackFX.Play();
                        _hitAbility.UseAbility();
                        OnCombatAbility?.Invoke();
                        yield return new WaitForSeconds(_attackInterval);
                    }
                }
            }
            yield return null;
        }
    }
}
