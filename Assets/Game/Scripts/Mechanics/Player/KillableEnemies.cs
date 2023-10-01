using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KillableEnemies : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"entered by {other.name}");
        if (other.CompareTag("Goon"))
        {
            GoonBase goon = other.GetComponent<GoonBase>();
            if (goon != null && goon.gameObject.activeSelf)
            {
                PlayerController.Instance.AddToKillableGoonsList(goon);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Goon"))
        {
            GoonBase goon = other.GetComponent<GoonBase>();
            if (goon != null && goon.gameObject.activeSelf)
            {
                PlayerController.Instance.RemoveFromKillableGoonsList(goon);
            }
        }
    }
}
