using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunker : MonoBehaviour
{
    private Transform shootHole;
    [SerializeField] private float shootForceMult;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private bool isAuto = false;
    
    private void Awake()
    {
        shootHole = transform.Find("ShootHole");
        if (isAuto) StartCoroutine(IntervalShootRoutine(1.0f));
    }

    private IEnumerator IntervalShootRoutine(float interval)
    {
        while (true)
        {
            ShootTarget();

            yield return new WaitForSeconds(interval);
        }
    }

    public Target ShootTarget(Stop stop = null)
    {
        GameObject newTarget = GameObject.Instantiate(targetPrefab, shootHole.position, Quaternion.identity);
        newTarget.GetComponent<Target>().Init(stop);
        newTarget.transform.LookAt(newTarget.transform.position + shootHole.up);
        newTarget.GetComponent<Rigidbody>().AddForce(shootHole.up * shootForceMult, ForceMode.Impulse);
        return newTarget.GetComponent<Target>();
    }
}
