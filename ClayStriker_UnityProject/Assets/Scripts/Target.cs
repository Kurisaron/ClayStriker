using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private bool isSmashed = false;
    [SerializeField] private int pointValue;

    private void OnTriggerEnter(Collider other)
    {
        if (!isSmashed && Utils.GetBaseTransform(other.gameObject.transform).gameObject.name.Contains("Bullet"))
        {
            GameManager.Instance.AddScore(pointValue);
        }

        isSmashed = true;
        Destroy(gameObject);
    }
}
