using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public int Score { get; private set; } = 0;

    [Header("Health and Score")]
    [SerializeField] Image[] playerLives;
    [SerializeField] int goonKillPoints = 1000;
    [SerializeField] Text scoreText;
    [SerializeField] Image image;
    [SerializeField] Color flashColor;

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

    [Header("Audio")]
    [SerializeField] AudioClip moveThruInventory;

    private ICombatAbility[] _goonInventory;
    private int _numAbilitiesAllowed = 3;
    private int _currAbilityIndex = 0;

    private int _numDashesAllowed = 5;
    private int _numShotsAllowed = 12;
    private int _numHammersAllowed = 3;
    private int _numPuttsAllowed = 1;

    private AudioSource _audioSource;

    Coroutine _currentFlashRoutine = null;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;

        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There should not be more than one UI Manager in a scene.");
        }

        // Populating inventory
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

        // Fill in ability information
        _numDashesAllowed = PlayerController.Instance.numDashesAllowed;
        _numShotsAllowed = PlayerController.Instance.numShotsAllowed;
        _numHammersAllowed = PlayerController.Instance.numHammersAllowed;
        _numPuttsAllowed = PlayerController.Instance.numPuttsAllowed;

        // Audio source
        _audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
        // Changing goon ability in  inventory
        if (Input.GetKeyDown(KeyCode.Space))
        {
            slotImages[_currAbilityIndex].texture = inventorySlotTx;
            _currAbilityIndex++;

            if (_currAbilityIndex >= _goonInventory.Length)
            {
                _currAbilityIndex = 0;
            }

            slotImages[_currAbilityIndex].texture = selectedInvSlotTx;
            PlayerController.Instance.SetCombatAbility(_goonInventory[_currAbilityIndex]);
            PlayFX(moveThruInventory);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
                    abilityNumText[i].text = _numShotsAllowed.ToString();
                    placed = true;
                }
                else if (goon is ShielderGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerShieldCombatAbility();
                    abilityImages[i].texture = shieldGoonTx;
                    abilityNumText[i].text = "1";
                    placed = true;
                }
                else if (goon is SpeedyGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerSpeedCombatAbility();
                    abilityImages[i].texture = speedGoonTx;
                    abilityNumText[i].text = _numDashesAllowed.ToString();
                    placed = true;
                }
                else if (goon is HammerGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerHammerCombatAbility();
                    abilityImages[i].texture = hammerGoonTx;
                    abilityNumText[i].text = _numHammersAllowed.ToString();
                    placed = true;
                }
                else if (goon is PutterGoon)
                {
                    _goonInventory[i] = PlayerController.Instance.GetPlayerPuttPuttCombatAbility();
                    abilityImages[i].texture = puttGoonTx;
                    abilityNumText[i].text = _numPuttsAllowed.ToString();
                    placed = true;
                }

                if (_currAbilityIndex == i)
                {
                    PlayerController.Instance.SetCombatAbility(_goonInventory[i]);
                }
            }
        }
    }

    public void RemoveGoonAbilityFromInventory()
    {
        _goonInventory[_currAbilityIndex] = new NoCombatAbility();
        abilityImages[_currAbilityIndex].texture = noAbilityTx;
        abilityNumText[_currAbilityIndex].text = "0";
        PlayerController.Instance.SetCombatAbility(_goonInventory[_currAbilityIndex]);
    }

    public void UpdateCurrentAbilityText(int newNum)
    {
        abilityNumText[_currAbilityIndex].text = newNum.ToString();
    }

    public void IncrementScore()
    {
        Score += goonKillPoints;
        scoreText.text = (string.Format("{0:000000}", Score));
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
        
        StartFlash(0.25f, 0.5f, flashColor);
    }

    public void PlayFX(AudioClip sfx)
    {
        if (_audioSource != null && sfx != null)
        {
            _audioSource.PlayOneShot(sfx, _audioSource.volume);
        }
        else
        {
            Debug.LogError("Must have audio component and give sfx.");
        }
    }

    public void StartFlash(float secondsForOneFlash, float maxAlpha, Color newColor)
    {
        image.color = newColor;
        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);

        if (_currentFlashRoutine != null)
        {
            StopCoroutine(_currentFlashRoutine);
        }

        _currentFlashRoutine = StartCoroutine(Flash(secondsForOneFlash, maxAlpha));
    }

    IEnumerator Flash(float secondsForOneFlash, float maxAlpha)
    {
        float flashInDuration = secondsForOneFlash / 2;
        for (float t = 0; t <= flashInDuration; t += Time.deltaTime)
        {
            Color colorThisFrame = image.color;
            colorThisFrame.a = Mathf.Lerp(0, maxAlpha, t / flashInDuration);
            image.color = colorThisFrame;
            yield return null;
        }

        float flashOutDuration = secondsForOneFlash / 2;
        for (float t = 0; t <= flashOutDuration; t += Time.deltaTime)
        {
            Color colorThisFrame = image.color;
            colorThisFrame.a = Mathf.Lerp(maxAlpha, 0, t / flashOutDuration);
            image.color = colorThisFrame;
            yield return null;
        }

        image.color = new Color32(0, 0, 0, 0);
    }
}
