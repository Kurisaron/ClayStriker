using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Utils
{
    public static Transform GetBaseTransform(this Transform child)
    {
        while (child.parent != null)
        {
            child = child.parent;
        }
        return child;
    }
    
}
