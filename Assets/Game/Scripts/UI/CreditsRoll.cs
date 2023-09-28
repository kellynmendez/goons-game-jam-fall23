using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    [SerializeField] private GameObject text;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float speed = 10;
    Vector3 velocity;
    
    // Update is called once per frame
    void Update()
    {
        text.transform.position = Vector3.SmoothDamp(text.transform.position, targetPosition, ref velocity, smoothTime, speed);
    }
}
