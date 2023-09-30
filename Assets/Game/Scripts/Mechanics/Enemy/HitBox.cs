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
                player.Hurt(this.transform.forward);
            }
        }
    }
}
