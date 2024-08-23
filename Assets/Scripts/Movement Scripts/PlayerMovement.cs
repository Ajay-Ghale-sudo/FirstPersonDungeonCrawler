using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement params")] 
    public bool smoothTransition = false;
    public float transitionSpeed = 10f;
    public float stepTime = 3f;
    public float transitionRotationSpeed = 500f;

    [Header("Camera params")] 
    public float xSens = .25f;
    public float ySens = 2f;
    private float xRotation;
    private float xRotationLimit = 45f;
    private float yRotation;
    private float yRotationLimit;

    private Vector3[] movePoints = new Vector3[4];
    private int currentPoint = 0;
    private bool isMoving = false;
    private bool isAiming = false;
    private Vector3 targetGridPos;
    private Vector3 prevTargetGridPos;
    private Vector3 targetRotation;
    private float elapsedTime;
    private int i = 0;

    public void RotateLeft() { if (AtRest)  targetRotation -= Vector3.up * 90f; }
    public void RotateRight() { if (AtRest)  targetRotation += Vector3.up * 90f; }
    public void MoveForward() { if (!IsObstructed(1) && AtRest && IsPathable(1)) isMoving = true; }
    public void MoveBackwards() { if (!IsObstructed(-1) && AtRest && IsPathable(-1)) isMoving = true; }
    public void Aim() { if (!isMoving) isAiming = true; } 
    public void tryInteract() { if (AtRest) FireInteractRay(); }


    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveToLocation();
        }

        if (!isMoving && !isAiming)
        {
            TurnToDirection();
        }
    }
    

    void MoveToLocation()
    {
        targetGridPos = movePoints[currentPoint];
        transform.position = Vector3.MoveTowards(transform.position, targetGridPos, transitionSpeed * Time.deltaTime);
        if (transform.position == targetGridPos)
        {
            currentPoint++;
            if (currentPoint >= movePoints.Length)
            {
                isMoving = false;
                currentPoint = 0;
            }
        }
    }

    void TurnToDirection()
    {
        if (targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
        if (targetRotation.y < 0f) targetRotation.y = 270f;

        if (!smoothTransition)
        {
            transform.rotation = Quaternion.Euler(targetRotation);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * transitionRotationSpeed);
        }

        /*if (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f)
        {
            isTurning = false;
        }*/
    }

    bool AtRest
    {
        get
        {
            if ((Vector3.Distance(transform.position, targetGridPos) < 0.05f) &&
                (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f))
                return true;
            else
            {
                return false;
            }
        }
    }

    
    private bool IsObstructed(float moveDir)
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + new Vector3(0, 2.5f, 0);;
        Vector3 rayDirection = transform.forward * moveDir;
        Debug.DrawRay(rayOrigin, rayDirection, Color.cyan, 3.0f);
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 4.0f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsPathable(float moveDir)
    {
        if (!isMoving)
        {
            Vector3 pointPos = transform.position;
            for (i = 0; i < 4; i++)
            {
                RaycastHit hit;
                Vector3 rayOrigin = pointPos + new Vector3(transform.forward.x * moveDir, 4.0f, transform.forward.z * moveDir);
                Vector3 rayDirection = -transform.up;
                if (Physics.Raycast(rayOrigin, rayDirection, out hit))
                {
                    if (hit.point.y - rayOrigin.y > 1.25 || hit.point.y - rayOrigin.y < -6.25)
                    {
                        return false;
                    }
                    movePoints[i] = hit.point;
                    pointPos = hit.point;
                    Debug.DrawLine(rayOrigin, hit.point, Color.blue, 3.0f);
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FireInteractRay()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 4))
        {
            if (hit.collider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactableInterface))
            {
                interactableInterface.Interact();
            }
        }
    }
}
