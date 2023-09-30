using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ShieldCombatAbility : ICombatAbility
{
    private MonoBehaviour _mono;
    private PlayerController _player;
    private GoonBase _goon;
    private float _inviDuration;

    public ShieldCombatAbility(MonoBehaviour mono, PlayerController player, GoonBase goon, float inviDuration)
    {
        _mono = mono;
        _player = player;
        _goon = goon;
        _inviDuration = inviDuration;
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
        }
        else
        {
            _goon.IsInvincible = true;
        }
        yield return new WaitForSeconds(_inviDuration);
        if (_player != null)
        {
            _player.IsInvincible = false;
        }
        else
        {
            _goon.IsInvincible = false;
        }
        AbilityUsedUp();
    }
}
