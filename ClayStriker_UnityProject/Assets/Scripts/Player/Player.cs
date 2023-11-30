using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    private CameraSettings cameraSettings { get => UIManager.Instance.cameraSettings; }
    private (Transform body, Transform camera, Transform gun) movingParts;
    private Func<Vector3> shootBearing;
    public bool IsRecoiling { get; private set; }

    private Transform shootPoint;
    [SerializeField, Tooltip("ONLY CHANGE IN PLAYER PREFAB")] private GameObject bulletPrefab;
    
    public override void Awake()
    {
        base.Awake();

        SetParts();
        SetBearing(Vector3.forward);
        IsRecoiling = false;
    }

    private void SetParts()
    {
        Transform body = transform.Find("Body");
        if (body == null)
        {
            Debug.LogError("No body found on player");
            return;
        }
        movingParts.body = body;

        Transform camera = body.Find("Camera");
        if (camera == null)
        {
            Debug.LogError("No camera found on player");
            return;
        }
        movingParts.camera = camera;

        Transform gun = camera.Find("Shotgun");
        if (gun == null)
        {
            Debug.LogError("No gun found on player");
            return;
        }
        movingParts.gun = gun;

        Transform point = gun.Find("ShootPoint");
        if (point == null)
        {
            Debug.LogError("No shoot point found on player");
            return;
        }
        shootPoint = point;
    }

    // Set the shoot bearing that restricts which way the camera can look
    public void SetBearing(Vector3 direction, Transform localOrigin = null)
    {
        if (localOrigin == null) localOrigin = transform;

        shootBearing = new Func<Vector3>(() => localOrigin.TransformDirection(direction));
        
        movingParts.body.LookAt(movingParts.body.position + new Vector3(shootBearing().x, 0.0f, shootBearing().z));
    }

    public void SetBearing(Func<Vector3> direction)
    {
        shootBearing = direction;

        movingParts.body.LookAt(movingParts.body.position + new Vector3(shootBearing().x, 0.0f, shootBearing().z));
    }

    //=================
    // INPUT FUNCTIONS
    //=================

    public void Look(Vector2 delta)
    {
        //Debug.Log("Player look delta is (" + delta.x.ToString() + ", " + delta.y.ToString() + ")");
        delta.y *= -1;
        Vector3 bearing = shootBearing();

        // Rotate left/right
        Transform body = movingParts.body;

        Vector2 from1 = new Vector2(body.forward.x, body.forward.z);
        Vector2 to1 = new Vector2(bearing.x, bearing.z);
        float angleFromBearing = Vector2.SignedAngle(from1, to1);
        //Debug.Log("Player angle from bearing is " + angleFromBearing.ToString());

        if (Mathf.Sign(angleFromBearing) == Mathf.Sign(delta.x))
        {
            delta.x = Mathf.Sign(delta.x) * Mathf.Min(Mathf.Abs(delta.x), cameraSettings.xLimit - Mathf.Abs(angleFromBearing));
        }
        body.Rotate(new Vector3(0.0f, delta.x * cameraSettings.turnSpeed, 0.0f), Space.Self);

        // Rotate up/down
        Transform camera = movingParts.camera;

        Vector3 from2 = camera.forward;
        Vector3 to2 = new Vector3(camera.forward.x, 0.0f, camera.forward.z);
        float angleFromHorizon = Vector3.SignedAngle(from2, to2, -camera.right);
        //Debug.Log("Player angle from horizon is " + angleFromHorizon.ToString());

        if (Mathf.Sign(angleFromHorizon) == Mathf.Sign(delta.y))
        {
            delta.y = Mathf.Sign(delta.y) * Mathf.Min(Mathf.Abs(delta.y), (Mathf.Sign(delta.y) > 0 ? cameraSettings.yLimitLower : cameraSettings.yLimitUpper) - Mathf.Abs(angleFromHorizon));
        }
        camera.Rotate(new Vector3(delta.y * cameraSettings.turnSpeed, 0.0f, 0.0f), Space.Self);
    }

    public void Shoot()
    {
        //Debug.Log("Player gun fired");
        GameObject newBullet = GameObject.Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        newBullet.transform.LookAt(newBullet.transform.position + shootPoint.forward);
        newBullet.GetComponent<Rigidbody>().AddForce(shootPoint.forward * 8.0f, ForceMode.Impulse);

        // Gun recoil
        StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        IsRecoiling = true;

        (float time, float duration) recoil = (0.0f, 0.05f);
        while (recoil.time <= recoil.duration)
        {
            yield return null;
            Look(Vector2.up * Time.deltaTime * (60.0f / cameraSettings.turnSpeed));
            recoil.time += Time.deltaTime;
        }

        IsRecoiling = false;
    }
}
