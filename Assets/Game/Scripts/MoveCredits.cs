using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCredits : MonoBehaviour
{
    public GameObject text;
    public Vector3 targetPosition;
    public float smoothTime;
    public float speed;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        text.transform.Translate(0f,-700f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        text.transform.position = Vector3.SmoothDamp(text.transform.position, targetPosition, 
                                                     ref velocity, smoothTime, speed);
    }
}
