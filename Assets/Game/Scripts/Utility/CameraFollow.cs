using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraFollow : MonoBehaviour
{
    private Vector3 _objectOffset;
    private PlayerController _player;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();

        // Creating offset between this position and object's position
        _objectOffset = transform.position - _player.transform.position;
    }

    private void LateUpdate()
    {
        // Updating position based on position of camera in relation to the player
        this.transform.position = _player.transform.position + _objectOffset;
    }
}
