using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
    }

    public void AllInputEnable()
    {
        playerInput.Player.OnFire.Enable();
        playerInput.Player.OnMove.Enable();
    }
    public void AllInputDisable()
    {
        playerInput.Player.OnFire.Disable();
        playerInput.Player.OnMove.Disable();
    }
}
