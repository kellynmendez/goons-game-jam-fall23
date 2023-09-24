using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability behavior abstraction that is used to define different
/// combat abilities that the player can have (and that each enemy
/// has specific to their character) so abilities can be swapped out
/// at runtime
/// </summary>
public interface ICombatAbility
{
    void UseAbility();
}
