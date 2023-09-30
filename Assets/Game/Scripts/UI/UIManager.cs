using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Health")]
    [SerializeField] Image[] playerLives;

    [Header("Inventory")]
    [SerializeField] Texture noAbilityTx;
    [SerializeField] Texture shootGoonTx;
    [SerializeField] Texture shieldGoonTx;
    [SerializeField] Texture speedGoonTx;
    [SerializeField] Texture hammerGoonTx;
    [SerializeField] Texture puttGoonTx;
    [SerializeField] Texture inventorySlotTx;
    [SerializeField] Texture selectedInvSlotTx;
    [SerializeField] RawImage[] abilityImages;
    [SerializeField] RawImage[] slotImages;
    [SerializeField] Text[] abilityNumText;

    private ICombatAbility[] _goonInventory;
    private int _numAbilitiesAllowed = 3;
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
            Debug.LogError("There should not be more than one UI Manager in a scene.");
        }

        _goonInventory = new ICombatAbility[_numAbilitiesAllowed];
        for (int i = 0; i < _numAbilitiesAllowed; i++)
        {
            _goonInventory[i] = new NoCombatAbility();
            abilityImages[_currAbilityIndex].texture = noAbilityTx;
            slotImages[_currAbilityIndex].texture = inventorySlotTx;
        }

        // Set first slot as the one selected automatically
        slotImages[0].texture = selectedInvSlotTx;

        // Enabling all hearts
        for (int i = 0; i < playerLives.Length; i++)
        {
            playerLives[i].enabled = true;
        }
    }

    private void Update()
    {
        // Changing goon ability in  inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            slotImages[_currAbilityIndex].texture = inventorySlotTx;
            _currAbilityIndex++;

            if (_currAbilityIndex >= _goonInventory.Length)
            {
                _currAbilityIndex = 0;
            }

            slotImages[_currAbilityIndex].texture = selectedInvSlotTx;
            PlayerController.Instance.SetCombatAbility(_goonInventory[_currAbilityIndex]);
        }
    }

    public void AddGoonAbilityToInventory(GoonBase goon)
    {
        bool placed = false;
        for (int i = 0; i < _goonInventory.Length && !placed; i++)
        {
            // If no goon in spot, fill spot with goon eaten
            if (_goonInventory[i] is NoCombatAbility)
            {
                if (goon is ShooterGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerShootCombatAbility();
                    abilityImages[i].texture = shootGoonTx;
                    placed = true;
                }
                else if (goon is ShielderGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerShieldCombatAbility();
                    abilityImages[i].texture = shieldGoonTx;
                    placed = true;
                }
                else if (goon is SpeedyGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerSpeedCombatAbility();
                    abilityImages[i].texture = speedGoonTx;
                    placed = true;
                }
                else if (goon is HammerGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerHammerCombatAbility();
                    abilityImages[i].texture = hammerGoonTx;
                    placed = true;
                }
                else if (goon is PutterGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerPuttPuttCombatAbility();
                    abilityImages[i].texture = puttGoonTx;
                    placed = true;
                }

                if (_currAbilityIndex == i)
                {
                    PlayerController.Instance.SetCombatAbility(_goonInventory[i]);
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
        _goonInventory[_currAbilityIndex] = new NoCombatAbility();
        abilityImages[_currAbilityIndex].texture = noAbilityTx;
        PlayerController.Instance.SetCombatAbility(_goonInventory[_currAbilityIndex]);
    }

    public void UpdateScore()
    {
        
    }

    public void PlayerHurt()
    {
        bool hurt = false;
        for (int i = playerLives.Length - 1; i >= 0 && !hurt; i--)
        {
            if (playerLives[i].enabled)
            {
                playerLives[i].enabled = false;
                hurt = true;
            }
        }
    }
}
