using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance;

    private Vector3 _objectOffset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Creating offset between this position and object's position
        _objectOffset = transform.position - PlayerController.Instance.transform.position;
    }

    private void LateUpdate()
    {
        // Updating position based on position of camera in relation to the player
        this.transform.position = PlayerController.Instance.transform.position + _objectOffset;
    }
}
