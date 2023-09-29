using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedyGoon : GoonBase
{
    [SerializeField] float _attackInterval = 2.5f;
    [SerializeField] Collider _hitCollider;

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
        while (true)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        _hitAbility.UseAbility();
                        yield return new WaitForSeconds(_attackInterval);
                    }
                }
            }
            yield return null;
        }
    }
}
