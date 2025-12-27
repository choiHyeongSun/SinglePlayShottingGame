using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetingController : MonoBehaviour
{
    [SerializeField] private LayerMask pickingMask;
    [SerializeField] private float rotSpeed;
    private Player player;
    private Vector2 mousePosition;
    public Vector3 pickingPosition { get; private set; }

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Start()
    {
        PlayerInput playerInput = player.playerInputController.playerInput;
        playerInput.Player.OnMouse.performed += OnMouse;
    }

    private void OnMouse(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }
    private void Update()
    {
        LookatToPickingPos();
    }
    private void FixedUpdate()
    {
        Picking();
    }

    private void LookatToPickingPos()
    {
        if (!player.playerStateController.canTurn)
        {
            return;
        }

        Vector3 direction = pickingPosition - transform.position;
        direction = new Vector3(direction.x, 0.0f, direction.z).normalized;

        Quaternion resultRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, resultRot, rotSpeed * Time.deltaTime);
    }

    private void Picking()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, pickingMask))
        {
            pickingPosition = hitInfo.point;
        }
    }
}
