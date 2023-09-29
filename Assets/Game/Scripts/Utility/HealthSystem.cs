using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public bool IsInvincible { get; set; } = false;
    public bool IsDead { get; set; } = false;

    [SerializeField] int lives = 1;
    [SerializeField] UnityEvent onHurt = null;

    internal int startNumLives;
    internal int LivesLeft;

    private void Awake()
    {
        startNumLives = lives;
        LivesLeft = startNumLives;
    }

    public void Hurt()
    {
        if (IsDead)
            return;

        if (IsInvincible)
            return;

        LivesLeft -= 1;

        onHurt.Invoke();

        if (LivesLeft <= 0)
        {
            Kill();
        }
    }

    public void ResetLives()
    {
        LivesLeft = startNumLives;
    }

    public void Kill()
    {
        //Debug.Log($"{gameObject} killed");

        PlayerController player = gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Kill();
        }
        else
        {
            GoonBase goon = gameObject.GetComponent<GoonBase>();
            if (goon != null)
            {
                goon.Kill();
            }
        }

        IsDead = true;
        LivesLeft = 0;
    }
}
