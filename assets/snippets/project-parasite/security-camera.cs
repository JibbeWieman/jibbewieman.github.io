using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject camBody;

    [SerializeField]
    private SceneTypeObject ST_Player;
    private GameObject player;

    public Light OnLight;

    [SerializeField]
    private float angle, range, height;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField, Range(0, 180)] // Limit max rotation range to 180 degrees
    private int maxRotateAngle; // In degrees

    private float currentAngle;
    private int rotateDirection = 1; // 1 for right, -1 for left
    private bool isWaiting;

    private Quaternion initialRotation;

    private void Start()
    {
        player = ST_Player.Objects[0];
        initialRotation = camBody.transform.localRotation;
    }


    private void Update()
    {
        RotateCameraBody();

        if (ConeCheck.CheckCone(camBody.transform.position + new Vector3(0, 0, 1), camBody.transform.forward, angle, range, height, LayerMask.NameToLayer("Player"), true)
            .Contains(player.GetComponent<Collider>()))
        {
            // something
            Debug.Log("Spotted player");
        }
    }

    #region CAMERA ROTATION
    private void RotateCameraBody()
    {
        if (isWaiting) return;

        // Update the current angle
        currentAngle += rotateSpeed * Time.deltaTime * rotateDirection;

        // Clamp the rotation and handle pausing
        if (Mathf.Abs(currentAngle) >= maxRotateAngle)
        {
            currentAngle = Mathf.Clamp(currentAngle, -maxRotateAngle, maxRotateAngle);
            rotateDirection *= -1; // Reverse the direction
            StartCoroutine(PauseRotation());
        }

        // Apply the rotation relative to the initial rotation
        camBody.transform.localRotation = initialRotation * Quaternion.Euler(0, currentAngle, 0);
    }

    private IEnumerator PauseRotation()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2f);
        isWaiting = false;
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        ConeCheck.DrawConeGizmo(camBody.transform.position + new Vector3(0, 0, -1), camBody.transform.forward, angle, range, height);
    }
    #endregion
}