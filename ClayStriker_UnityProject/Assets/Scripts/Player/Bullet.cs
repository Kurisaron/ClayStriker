using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Player.Instance.gameObject.transform.position) > 30.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Gun") Destroy(gameObject);
    }
}
