using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCombatAbility : ICombatAbility
{
    private MonoBehaviour _mono;
    private PlayerController _player;
    private GoonBase _goon;
    private float _inviDuration;
    ParticleSystem _invincibilityVFX;

    public ShieldCombatAbility(MonoBehaviour mono, PlayerController player, GoonBase goon, 
        float inviDuration, ParticleSystem invincibilityVFX)
    {
        _mono = mono;
        _player = player;
        _goon = goon;
        _inviDuration = inviDuration;
        _invincibilityVFX = invincibilityVFX;
    }

    public void UseAbility()
    {
        _mono.StartCoroutine(MakeInvincible());
    }

    public void AbilityUsedUp()
    {
        UIManager.Instance.RemoveGoonAbilityFromInventory();
    }

    private IEnumerator MakeInvincible()
    {
        if (_player != null)
        {
            _player.IsInvincible = true;
            _invincibilityVFX.Play();
        }
        yield return new WaitForSeconds(_inviDuration);
        if (_player != null)
        {
            _player.IsInvincible = false;
            _invincibilityVFX.Stop();
        }
        AbilityUsedUp();
    }
}
