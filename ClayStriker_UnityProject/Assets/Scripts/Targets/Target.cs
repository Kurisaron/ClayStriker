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
        if (Vector3.Distance(transform.position, Player.Instance.gameObject.transform.position) > 100.0f)
        {

            if (parentStop != null)
            {
                //Debug.LogError("Parent stop has " + (parentStop.activeTargets.Count - 1).ToString() + " targets left");
                parentStop.activeTargets.Remove(this);
                parentStop = null;
            }

            // Line below commented out as per programmer discretion, realized it was unnecessary to play VFX when the targets are too far from the player
            // VFXManager.Instance.TargetBreak(transform.position, GetComponent<Rigidbody>().velocity.normalized);
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Transform otherBaseGO = other.transform.GetBaseTransform();
        if (otherBaseGO.name.Contains("Bunker") || otherBaseGO.name.Contains("Spawner")) return;

        if (parentStop != null)
        {
            //Debug.LogError("Parent stop has " + (parentStop.activeTargets.Count - 1).ToString() + " targets left");
            parentStop.activeTargets.Remove(this);
            parentStop = null;
        }

        if (!isSmashed && other.gameObject.transform.GetBaseTransform().gameObject.name.Contains("Bullet"))
        {
            GameManager.Instance.AddScore(pointValue);
        }

        isSmashed = true;
        VFXManager.Instance.TargetBreak(transform.position, GetComponent<Rigidbody>().velocity.normalized);
        Destroy(gameObject);
    }
}
