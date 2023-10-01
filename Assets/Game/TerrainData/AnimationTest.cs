using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] Animator animator;

    void PlayAnimation()
    {
        animator.SetTrigger("Walk");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("walking");
            PlayAnimation();
        }
    }
}
