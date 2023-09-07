using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Returns the root/base transform of this one
    /// </summary>
    /// <param name="child">The tranform being checked</param>
    /// <returns>The base transform for the given transform</returns>
    public static Transform GetBaseTransform(this Transform child)
    {
        while (child.parent != null)
        {
            child = child.parent;
        }
        return child;
    }
    
    /// <summary>
    /// Set the world position, rotation, and the parent of the provided transform
    /// </summary>
    /// <param name="transform">The transform provided</param>
    /// <param name="position">The new world position</param>
    /// <param name="rotation">The new world rotation</param>
    /// <param name="parent">The new parent for the transform</param>
    public static void SetPositionRotationAndParent(this Transform transform, Vector3 position, Quaternion rotation, Transform parent)
    {
        transform.SetPositionAndRotation(position, rotation);
        transform.parent = parent;
    }
}
