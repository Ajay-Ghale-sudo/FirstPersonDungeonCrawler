using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInput : MonoBehaviour
{
    [Header("Keyboard Bindings")] 
    public KeyCode forward;
    public KeyCode back;
    public KeyCode left;
    public KeyCode right;
    public KeyCode turnLeft;
    public KeyCode turnRight;
    public KeyCode interactKey;

    [Tooltip("Integer that corresponds to mouse buttons. 0 is left click, 1 right click, and 2 middle click.")]
    [Range(0, 2)][SerializeField]
    public int aim;

    private PlayerMovement controller;
    private CameraManualAim cameraScript;
    private UIManager ui;

    private void Awake()
    {
        controller = GetComponent<PlayerMovement>();
        cameraScript = GetComponentInChildren<CameraManualAim>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(forward))  controller.MoveForward();
        if (Input.GetKeyUp(back)) controller.MoveBackwards();
        if (Input.GetKeyUp(turnLeft)) controller.RotateLeft();
        if (Input.GetKeyUp(turnRight)) controller.RotateRight();
        if (Input.GetKeyUp(interactKey)) controller.tryInteract();
        if (Input.GetMouseButtonDown(aim))  cameraScript.Freelook(); ui.EnableCursorAndTrack(); 
        if (Input.GetMouseButtonUp(aim)) cameraScript.StopFreelooking(); ui.DisableCursorAndStopTrack();
    }
}
