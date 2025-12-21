using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    private CharacterController characterController { get; set; }
    private Player player;
    private bool isMove = true;
    private Vector2 moveInput;
    [field: SerializeField] private float moveSpeed { get; set; }
    private void Awake()
    {
        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        PlayerInput playerInput = player.playerInputController.playerInput;
        playerInput.Player.OnMove.performed += OnMove;
        playerInput.Player.OnMove.canceled += OnMoveCancel;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        isMove = true;
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        isMove = false;
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {


        player.animator.SetFloat("AxisX", moveInput.x, 0.25f ,Time.deltaTime);
        player.animator.SetFloat("AxisZ", moveInput.y, 0.25f, Time.deltaTime);

        if (!isMove) return;

        Transform cameraTrans = Camera.main.transform;

        Vector3 cameraForwardToVector2 = cameraTrans.forward;
        cameraForwardToVector2.y = 0;
        Vector3 cameraRightdToVector2 = cameraTrans.right;
        cameraRightdToVector2.y = 0;

        cameraForwardToVector2 = cameraForwardToVector2.normalized;
        cameraRightdToVector2 = cameraRightdToVector2.normalized;



        Vector3 dirForward = cameraForwardToVector2 * moveInput.y;
        Vector3 dirRight = cameraRightdToVector2 * moveInput.x;

        Vector3 dir = Vector3.Normalize(dirForward + dirRight) * moveSpeed * Time.deltaTime;
        characterController.Move(dir);

    }
}
