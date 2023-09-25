using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
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

        LivesLeft -= 1;

        Debug.Log($"{gameObject} was hurt and lives are {LivesLeft}");

        onHurt.Invoke();

        if (LivesLeft <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log("player killed");

        PlayerController player = gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.IsDead = true;
        }
        else
        {
            GoonBase goon = gameObject.GetComponent<GoonBase>();
            if (goon != null)
            {
                goon.IsDead = true;
            }
        }
        
        IsDead = true;
        LivesLeft = 0;
        onDeath.Invoke();

        gameObject.SetActive(false);
    }
}
