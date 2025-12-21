using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetingController : MonoBehaviour
{
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
        Vector3 direction = pickingPosition - transform.position;
        direction.y = 0;
        Vector3 lookAtPos = transform.position + direction;
        transform.LookAt(lookAtPos);
    }

    private void Picking()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            pickingPosition = hitInfo.point;
        }
    }
}
