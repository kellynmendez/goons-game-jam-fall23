using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    [SerializeField] private GameObject text;
    [SerializeField] private float endYOffset = 1200f;
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float speed = 10;
    Vector3 velocity;

    private Vector3 targetPosition;

    private void Awake()
    {
        targetPosition = text.transform.position + new Vector3(0, endYOffset, 0);
    }

    void Update()
    {
        text.transform.position = Vector3.SmoothDamp(text.transform.position, targetPosition, ref velocity, smoothTime, speed);
    }
}
