
using System;
using System.Collections;
using UnityEngine;

public class CameraManualAim : MonoBehaviour
{

    public float Sensitivity { get { return sensitivity; } set { sensitivity = value; }}
    private PlayerMovement playermovement;

    [Header("Camera settings.")]
    [Range(0.1f, 9f)][SerializeField] 
    float sensitivity = 2.0f;

    [Tooltip("Limits vertical camera rotation. Prevents 6DOF that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] 
    public float yRotationLimit = 88f;
    [Tooltip("Limits horizontal camera rotation while aiming, preventing the player from aiming behind them.")]
    [Range(0f, 120f)]
    public float xRotationLimit = 70f;
    [Tooltip("Amount of zoom that should be applied when freeaim is started.")]
    [Range(0, 10)][SerializeField]
    public float zoomAmount = 5f;

    [Tooltip("Enables camera smoothing.")]
    public bool useSmoothing;
    private bool isFreelooking = false;
    private Vector2 rotation;
    private Quaternion currentRotation;
    private Quaternion newQuatRotation;
    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";

    private void Awake()
    {
        playermovement = GetComponentInParent<PlayerMovement>();
    }

    public void Freelook() { isFreelooking = true; print("Started freelooking."); }
    public void StopFreelooking() { isFreelooking = false; print("Stopped freelooking."); OrientToForwardsDirection(); }

    private void Update()
    {
        if (isFreelooking)
        {
            FreeAim();
        }
        if (!isFreelooking)
        {
            currentRotation = transform.localRotation;
        }

    }

    private void FreeAim()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        rotation.x = Mathf.Clamp(rotation.x, -xRotationLimit, xRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        var newRotation = xQuat * yQuat;

        if (useSmoothing)
        {
            // Smoothing can cause camera to rotate on Z axis.
            transform.localRotation = Quaternion.Lerp(currentRotation, newRotation, Time.deltaTime * sensitivity);
        }
        else
        {
            transform.localRotation = newRotation;
            currentRotation = newRotation;
        }
    }

    //Previous implementation. Still might use later.
    /*private void OrientToNearestCardinalDirection()
    {

        Vector3 currentEulerAngles = currentRotation.eulerAngles;
        float newYawAngle = Mathf.Round(currentEulerAngles.y / 90) * 90;
        newQuatRotation = Quaternion.Euler(0, newYawAngle, 0);

        StartCoroutine(SmoothRotateToCardinalDirection(newQuatRotation));

    }*/

    private void OrientToForwardsDirection()
    {
        Vector3 currentEulerAngles = currentRotation.eulerAngles;
        float newYawAngle = 0;
        Quaternion newQuatRotation = Quaternion.Euler(0, newYawAngle, 0);
        StartCoroutine(SmoothRotateToCardinalDirection(newQuatRotation));
    }
    private IEnumerator SmoothRotateToCardinalDirection(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.localRotation;
        float elapsedTime = 0f;
        float interpolationDuration = 0.2f;
        while (elapsedTime < interpolationDuration)
        {
            float interpolationValue = elapsedTime / interpolationDuration;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, interpolationValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = targetRotation;
        currentRotation = targetRotation;
        rotation.x = newQuatRotation.eulerAngles.y;
        rotation.y = newQuatRotation.eulerAngles.x;
    }
}
