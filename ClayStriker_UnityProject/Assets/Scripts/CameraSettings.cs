using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraSettings
{
    [Tooltip("The speed for turning the camera. Default = 0.5")]
    public float turnSpeed;
    [Tooltip("The angle the camera can rotate below the horizon")]
    public float yLimitLower;
    [Tooltip("The angle the camera can rotate above the horizon")]
    public float yLimitUpper;
    [Tooltip("The angle the camera can rotate from its bearing")]
    public float xLimit;
}
