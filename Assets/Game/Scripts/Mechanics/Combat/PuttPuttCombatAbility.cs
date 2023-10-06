using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuttPuttCombatAbility : ICombatAbility
{
    private PlayerController _player;
    private List<GoonBase> _killableGoons;
    private ParticleSystem _puttRing;

    public PuttPuttCombatAbility(PlayerController player, List<GoonBase> killableGoons, ParticleSystem puttRing)
    {
        _player = player;
        _killableGoons = killableGoons;
        _puttRing = puttRing;
    }

    public void UseAbility()
    {
        _player.IsUsingGoonAbility = true;
        _puttRing.Play();

        if (_killableGoons.Count == 0)
        {
            Debug.Log("Nothing to putt putt!");
        }
        else
        {
            // Damage all goons in killable goons list
            for (int i = 0; i < _killableGoons.Count; i++)
            {
                GoonBase goonToKill = _killableGoons[i].GetComponent<GoonBase>();
                goonToKill.Hurt();
            }
        }

        _player.IsUsingGoonAbility = false;
    }
}