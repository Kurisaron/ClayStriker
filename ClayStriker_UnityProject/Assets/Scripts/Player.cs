using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private CameraSettings cameraSettings;
    private (Transform body, Transform camera, Transform gun) movingParts;
    private Func<Vector3> shootBearing;
    
    public override void Awake()
    {
        base.Awake();

        SetMovingParts();
    }

    private void SetMovingParts()
    {
        Transform body = transform.Find("Body");
        if (body == null)
        {
            Debug.LogError("No body found on player");
            return;
        }
        movingParts.body = body;

        Transform camera = body.Find("Camera");
        if (camera == null)
        {
            Debug.LogError("No camera found on player");
            return;
        }
        movingParts.camera = camera;

        Transform gun = camera.Find("Gun");
        if (gun == null)
        {
            Debug.LogError("No gun found on player");
            return;
        }
        movingParts.gun = gun;
    }

    public void Look(Vector2 delta)
    {
        //Debug.Log("Player look delta is (" + delta.x.ToString() + ", " + delta.y.ToString() + ")");
        Vector3 bearing = shootBearing();

        // Rotate left/right

        // Rotate up/down
    }

    public void Shoot()
    {
        //Debug.Log("Player gun fired");
    }
}
