using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InFrontKillableEnemies : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goon"))
        {
            GoonBase goon = other.GetComponent<GoonBase>();
            if (goon != null && goon.gameObject.activeSelf)
            {
                PlayerController.Instance.AddToKillableGoonsInFrontList(goon);
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
                PlayerController.Instance.RemoveFromKillableGoonsInFrontList(goon);
            }
        }
    }
}
