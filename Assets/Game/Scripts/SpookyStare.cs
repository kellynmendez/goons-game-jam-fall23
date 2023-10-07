using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpookyStare : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Start()
    {
        animator.Play("spookySTare");
    }
}
