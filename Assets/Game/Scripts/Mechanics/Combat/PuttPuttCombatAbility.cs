using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player only uses this, not putt putt goon (putt putt's combat ability is different than
/// the player's)
/// </summary>
public class PuttPuttCombatAbility : ICombatAbility
{
    private PlayerController _player;
    private ParticleSystem _puttPuttRing;

    public PuttPuttCombatAbility(PlayerController player, ParticleSystem puttPuttRing)
    {
        _player = player;
        _puttPuttRing = puttPuttRing;
    }

    public void UseAbility()
    {
        _player.IsUsingGoonAbility = true;

        if (_puttPuttRing != null)
        {
            _puttPuttRing.Play();
        }
        else
        {
            Debug.LogError("No particle was given to use putt putt ability.");
        }

        _player.IsUsingGoonAbility = false;
    }
}
