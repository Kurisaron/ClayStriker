using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Transform GetBaseTransform(Transform child)
    {
        while (child.parent != null)
        {
            child = child.parent;
        }
        return child;
    }
}
