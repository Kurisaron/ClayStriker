using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private bool isSmashed = false;
    [SerializeField] private int pointValue;
    private Stop parentStop;

    public void Init(Stop stop = null)
    {
        parentStop = stop;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Player.Instance.gameObject.transform.position) > 30.0f)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetBaseTransform().gameObject.name.Contains("Bunker")) return;
        
        if (!isSmashed && other.gameObject.transform.GetBaseTransform().gameObject.name.Contains("Bullet"))
        {
            GameManager.Instance.AddScore(pointValue);
        }
        if (parentStop != null) parentStop.activeTargets.Remove(this);

        isSmashed = true;
        Destroy(gameObject);
    }
}
