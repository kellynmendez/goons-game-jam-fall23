using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutterGoon : GoonBase
{
    [Header("Putt Putt Settings")]
    [SerializeField] float _attackInterval = 2.5f;
    [SerializeField] float _pauseBeforeAttack = 0.5f;
    [SerializeField] Collider _hitCollider;

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
        StartCoroutine(Hit());
    }

    private IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (agent.isStopped && (agent.remainingDistance <= agent.stoppingDistance))
            {
                yield return new WaitForSeconds(_pauseBeforeAttack);
                if (!IsDead)
                {
                    // Using ability and invoking unity event
                    _animator.Play(ATTACK_ANIM);
                    yield return new WaitForSeconds(0.1f);
                    OnCombatAbility?.Invoke();
                    yield return new WaitForSeconds(_attackInterval);
                }
            }
            yield return null;
        }
    }
}
