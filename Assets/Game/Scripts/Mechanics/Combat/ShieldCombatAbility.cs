using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCombatAbility : ICombatAbility
{
    private MonoBehaviour _mono;
    private HealthSystem _healthSys;
    private float _inviDuration;

    public ShieldCombatAbility(MonoBehaviour mono, HealthSystem healthSys, float inviDuration)
    {
        _mono = mono;
        _healthSys = healthSys;
        _inviDuration = inviDuration;
    }

    public void UseAbility()
    {
        _mono.StartCoroutine(MakeInvincible());
    }

    private IEnumerator MakeInvincible()
    {
        _healthSys.IsInvincible = true;
        yield return new WaitForSeconds(_inviDuration);
        _healthSys.IsInvincible = false;
    }
}
