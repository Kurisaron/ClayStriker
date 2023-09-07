using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunker : MonoBehaviour
{
    private Transform shootHole;
    [SerializeField] private float shootForceMult;
    [SerializeField] private GameObject targetPrefab;
    
    private void Awake()
    {
        shootHole = transform.Find("ShootHole");
        if (shootHole == null) Debug.LogError("Bunker shoot hole not found");
        else StartCoroutine(IntervalShootRoutine(2.0f));
    }

    private IEnumerator IntervalShootRoutine(float interval)
    {
        while (true)
        {
            ShootTarget();

            yield return new WaitForSeconds(interval);
        }
    }

    private void ShootTarget()
    {
        GameObject newTarget = GameObject.Instantiate(targetPrefab, shootHole.position, Quaternion.identity);
        newTarget.transform.LookAt(newTarget.transform.position + shootHole.up);
        newTarget.GetComponent<Rigidbody>().AddForce(shootHole.up * shootForceMult, ForceMode.Impulse);
    }
}