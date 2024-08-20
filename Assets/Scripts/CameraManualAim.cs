
using System;
using System.Collections;
using UnityEngine;

public class CameraManualAim : MonoBehaviour
{
    public Camera camera;

    public float Sensitivity { get { return sensitivity; } set { sensitivity = value; }}

    [Header("Camera settings.")]
    [Range(0.1f, 9f)][SerializeField] 
    float sensitivity = 2.0f;

    [Tooltip("Limits vertical camera rotation. Prevents 6DOF that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] 
    public float yRotationLimit = 88f;

    [Tooltip("Enables camera smoothing.")]
    public bool useSmoothing;
    private bool isFreelooking = false;
    private Vector2 rotation;
    private Quaternion currentRotation;
    private Quaternion newQuatRotation;
    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";

    public void Freelook() { isFreelooking = true; print("Started freelooking."); }
    public void StopFreelooking() { isFreelooking = false; print("Stopped freelooking."); OrientToNearestCardinalDirection(); }

    private void Update()
    {
        if (isFreelooking)
        {
            FreeAim();
        }
        if (!isFreelooking)
        {
            currentRotation = camera.transform.localRotation;
        }

    }

    private void FreeAim()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        var newRotation = xQuat * yQuat;

        if (useSmoothing)
        {
            // Smoothing can cause camera to rotate on Z axis.
            camera.transform.localRotation = Quaternion.Lerp(currentRotation, newRotation, Time.deltaTime * sensitivity);
        }
        else
        {
            camera.transform.localRotation = newRotation;
            currentRotation = newRotation;
        }
    }

    private void OrientToNearestCardinalDirection()
    {

        Vector3 currentEulerAngles = currentRotation.eulerAngles;
        float newYawAngle = Mathf.Round(currentEulerAngles.y / 90) * 90;
        newQuatRotation = Quaternion.Euler(0, newYawAngle, 0);
        StartCoroutine(SmoothRotateToCardinalDirection(newQuatRotation));        

    }
    private IEnumerator SmoothRotateToCardinalDirection(Quaternion targetRotation)
    {
        Quaternion startRotation = camera.transform.localRotation;
        float elapsedTime = 0f;
        float interpolationDuration = 0.2f;
        while (elapsedTime < interpolationDuration)
        {
            float interpolationValue = elapsedTime / interpolationDuration;
            camera.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, interpolationValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camera.transform.localRotation = targetRotation;
        currentRotation = targetRotation;
        rotation.x = newQuatRotation.eulerAngles.y;
        rotation.y = newQuatRotation.eulerAngles.x;
    }
}
