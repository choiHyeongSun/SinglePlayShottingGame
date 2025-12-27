using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollingSpeed;

    private CharacterController characterController { get; set; }
    private Player player;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    public Vector3 moveDirection { get; private set; } 

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
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void Update()
    {

        UpdateMoveDirection();
        Move();
        RollingMove();
    }

    private void UpdateMoveDirection()
    {
        if (!player.playerStateController.canMove)
        {
            return;
        }
        
        Transform cameraTrans = Camera.main.transform;

        Vector3 cameraForwardToVector2 = cameraTrans.forward;
        cameraForwardToVector2.y = 0;
        Vector3 cameraRightdToVector2 = cameraTrans.right;
        cameraRightdToVector2.y = 0;

        cameraForwardToVector2 = cameraForwardToVector2.normalized;
        cameraRightdToVector2 = cameraRightdToVector2.normalized;

        Vector3 dirForward = cameraForwardToVector2 * moveInput.y;
        Vector3 dirRight = cameraRightdToVector2 * moveInput.x;

        moveDirection = Vector3.Normalize(dirForward + dirRight);
    }

    private void Move()
    {
        if (!player.playerStateController.canMove)
        {
            return;
        }

        moveVelocity.x = Vector3.Dot(moveDirection, transform.right);
        moveVelocity.y = Vector3.Dot(moveDirection, transform.forward);

        player.animator.SetFloat("AxisX", moveVelocity.x, 0.1f, Time.deltaTime);
        player.animator.SetFloat("AxisZ", moveVelocity.y, 0.1f, Time.deltaTime);

        Vector3 dir = moveDirection * moveSpeed * Time.deltaTime;
        characterController.Move(dir);
    }

    private void RollingMove()
    {
        if (!player.playerRollingController.isRolling)
        {
            return;
        }

        Vector3 dir = moveDirection * rollingSpeed * Time.deltaTime;
        characterController.Move(dir);
    }
}
