using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Utils
{
    public static Color RandomColor() => new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);

    //============
    // EXTENSIONS
    //============
    
    public static float AngleToRadians(this float angle)
    {
        return angle / 180 * Mathf.PI;
    }
    
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

    /// <summary>
    /// Switch two elements in a list for each other based on their index.
    /// </summary>
    /// <typeparam name="T">The type of the objects stored in the list.</typeparam>
    /// <param name="list">The list being switched.</param>
    /// <param name="firstElement">The first element being switched.</param>
    /// <param name="secondElement">The second element being switched.</param>
    public static List<T> Switch<T>(this List<T> list, T firstElement, T secondElement)
    {
        int firstIndex = list.FindIndex(element => element.Equals(firstElement));
        int secondIndex = list.FindIndex(element => element.Equals(secondElement));

        list[firstIndex] = secondElement;
        list[secondIndex] = firstElement;

        Debug.Log("Switch complete");
        return list;
    }
}
