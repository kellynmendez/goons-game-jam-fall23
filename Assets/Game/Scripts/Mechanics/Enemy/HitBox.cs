using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                Debug.Log("Player is hurt");
                HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                playerHealth.Hurt();
            }
        }
    }
}
