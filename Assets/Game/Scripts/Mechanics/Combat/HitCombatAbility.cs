using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Only goons use this ability to attack the player
/// </summary>
public class HitCombatAbility : ICombatAbility
{
    private MonoBehaviour _mono;
    private Collider _collider;

    public HitCombatAbility(MonoBehaviour mono, Collider collider)
    {
        _mono = mono;
        _collider = collider;
    }

    public void UseAbility()
    {
        _mono.StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        _collider.enabled = true;
        yield return new WaitForSeconds(0.05f);
        _collider.enabled = false;
    }
}
