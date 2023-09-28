using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InCircleKillableEnemies : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goon"))
        {
            GoonBase goon = other.GetComponent<GoonBase>();
            if (goon != null && goon.gameObject.activeSelf)
            {
                PlayerController.Instance.AddToKillableGoonsInCircleList(goon);
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
                PlayerController.Instance.RemoveFromKillableGoonsInCircleList(goon);
            }
        }
    }
}
