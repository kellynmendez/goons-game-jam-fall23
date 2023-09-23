using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraFollow : MonoBehaviour
{
    private Vector3 _objectOffset;

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
