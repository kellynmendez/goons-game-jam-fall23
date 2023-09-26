using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public bool IsInvincible { get; set; } = false;

    [SerializeField] int lives = 1;
    [SerializeField] UnityEvent onHurt = null;
    [SerializeField] UnityEvent onDeath = null;

    internal int LivesLeft;
    internal bool IsDead = false;

    private void Awake()
    {
        LivesLeft = lives;
    }

    public void Hurt()
    {
        if (IsDead)
            return;

        if (IsInvincible)
            return;

        LivesLeft -= 1;

        Debug.Log($"{gameObject} hurt, lives = {LivesLeft}");

        onHurt.Invoke();

        if (LivesLeft <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log($"{gameObject} killed");

        PlayerController player = gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.IsDead = true;
            player.enabled = false;
        }
        else
        {
            GoonBase goon = gameObject.GetComponent<GoonBase>();
            if (goon != null)
            {
                goon.Deactivate();
            }
        }

        IsDead = true;
        LivesLeft = 0;
        onDeath.Invoke();
    }
}
