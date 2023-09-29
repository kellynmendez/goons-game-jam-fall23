using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    private ICombatAbility[] _goonAbilities;
    private int _currAbilityIndex = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There should not be more than one inventory in a scene.");
        }

        _goonAbilities = new ICombatAbility[3];
        for (int i = 0; i < _goonAbilities.Length; i++)
        {
            _goonAbilities[i] = new NoCombatAbility();
        }
    }
    private void Update()
    {
        // Changing goon ability in  inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _currAbilityIndex++;

            if (_currAbilityIndex >= _goonAbilities.Length)
            {
                _currAbilityIndex = 0;
            }

            PlayerController.Instance.SetCombatAbility(_goonAbilities[_currAbilityIndex]);
        }
    }

    public void AddGoonAbilityToInventory(GoonBase goon)
    {
        for (int i = 0; i < _goonAbilities.Length; i++)
        {
            // If no goon in spot, fill spot with goon eaten
            if (_goonAbilities[i] is NoCombatAbility)
            {
                if (goon is ShooterGoon)
                {
                    _goonAbilities[i] = PlayerController.Instance.GetPlayerShootCombatAbility();
                }
                else if (goon is ShielderGoon)
                {
                    _goonAbilities[i] = PlayerController.Instance.GetPlayerShieldCombatAbility();
                }
                else if (goon is SpeedyGoon)
                {
                    _goonAbilities[i] = PlayerController.Instance.GetPlayerSpeedCombatAbility();
                }
                else if (goon is HammerGoon)
                {
                    _goonAbilities[i] = PlayerController.Instance.GetPlayerHammerCombatAbility();
                }
                else if (goon is PutterGoon)
                {
                    _goonAbilities[i] = PlayerController.Instance.GetPlayerPuttPuttCombatAbility();
                }

                if (_currAbilityIndex == i)
                {
                    PlayerController.Instance.SetCombatAbility(_goonAbilities[i]);
                }
            }
            else
            {
                Debug.Log("Inventory full!");
            }
        }
    }

    public void RemoveGoonAbilityFromInventory()
    {
        _goonAbilities[_currAbilityIndex] = new NoCombatAbility();
        PlayerController.Instance.SetCombatAbility(_goonAbilities[_currAbilityIndex]);
    }
}
