using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraSettings
{
    [Tooltip("The angle the camera can rotate below the horizon")]
    public float yLimit_Lower;
    [Tooltip("The angle the camera can rotate above the horizon")]
    public float yLimit_Upper;
    [Tooltip("The angle the camera can rotate from its bearing")]
    public float xLimit;
}
