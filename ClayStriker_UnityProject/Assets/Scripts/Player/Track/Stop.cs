using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : MonoBehaviour
{
    public Track track;

    public Stop Init(Track t)
    {
        track = t;
        
        return this;
    }
}
