using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerCombatAbility : ICombatAbility
{
    PlayerController _player;
    List<GoonBase> _killableGoons;

    public HammerCombatAbility(PlayerController player, List<GoonBase> killableGoons)
    {
        _player = player;
        _killableGoons = killableGoons;
    }

    public void UseAbility()
    {
        _player.IsUsingGoonAbility = true;

        if (_killableGoons.Count == 0)
        {
            Debug.Log("Nothing to hammer!");
        }
        else
        {
            // Damage all goons in killable goons list
            for (int i = 0; i < _killableGoons.Count; i++)
            {
                HealthSystem health = _killableGoons[i].GetComponent<HealthSystem>();
                health.Hurt();
            }
        }

        _player.IsUsingGoonAbility = false;
    }
}
