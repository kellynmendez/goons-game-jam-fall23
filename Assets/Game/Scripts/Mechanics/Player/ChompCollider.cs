using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChompCollider : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"{other.gameObject.name} entered");
        if (other.CompareTag("Goon"))
        {
            GoonBase goon = other.gameObject.GetComponent<GoonBase>();
            if (goon != null && goon.gameObject.activeSelf)
            {
                PlayerController.Instance.AddGoonToChompList(goon);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"{other.gameObject.name} exited");
        if (other.CompareTag("Goon"))
        {
            GoonBase goon = other.gameObject.GetComponent<GoonBase>();
            if (goon != null && goon.gameObject.activeSelf)
            {
                PlayerController.Instance.RemoveGoonFromChompList(goon);
            }
        }
    }
}
