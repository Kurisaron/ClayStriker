using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Track : Singleton<Track>
{
    //[SerializeField] private GameObject playerPrefab;
    
    // Set of stops for player to stop at
    public List<Stop> stops;

    public override void Awake()
    {
        base.Awake();

        
    }


}
