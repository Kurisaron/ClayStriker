using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputEvents : Singleton<InputEvents>
{
    //===========
    // VARIABLES
    //===========

    private InputState inputState;
    private Action<Vector2> lookEvent;
    private Action fireEvent;

    //=================
    // UNITY FUNCTIONS
    //=================

    public override void Awake()
    {
        base.Awake();

        SetInputState(inputState = InputState.Game);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) SetCursor();
    }

    //==============
    // INPUT EVENTS
    //==============

    // Runs the current look event when the mouse is moved
    public void Look(InputAction.CallbackContext context)
    {
        if (lookEvent == null)
        {
            Debug.LogError("Input Look Event is null reference");
            return;
        }

        lookEvent(context.ReadValue<Vector2>());
    }

    // Runs the current fire event when the left mouse is clicked
    public void Fire(InputAction.CallbackContext context)
    {
        if (fireEvent == null)
        {
            Debug.LogError("Input Fire Event is null reference");
            return;
        }

        if (context.started) fireEvent();
    }

    public void TestBearing(InputAction.CallbackContext context)
    {
        if (fireEvent != FireGun || !context.started) return;

        Vector2 direction = context.ReadValue<Vector2>();
        Debug.Log("Test bearing direction is (" + direction.x.ToString() + ", " + direction.y.ToString() + ")");
        Player.Instance.SetBearing(new Vector3(direction.x, 0.0f, direction.y));
    }

    //==============
    // INPUT STATES
    //==============

    // Used to switch input between playing the game and operating menus
    public void SetInputState(InputState state)
    {
        inputState = state;
        SetCursor();
        switch (state)
        {
            case InputState.Game:
                lookEvent = MoveCamera;
                fireEvent = FireGun;
                break;
            case InputState.Menu:
                lookEvent = MoveCursor;
                fireEvent = ClickMenu;
                break;
            default:
                break;
        }
    }

    private void SetCursor()
    {
        switch (inputState)
        {
            case InputState.Game:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case InputState.Menu:
                Cursor.lockState = CursorLockMode.Confined;
                break;
            default:
                break;
        }
    }

    // GAME: Move the player camera around
    private void MoveCamera(Vector2 delta) => Player.Instance.Look(delta);

    // GAME: Fire the gun
    private void FireGun() => Player.Instance.Shoot();

    // MENU: Move the cursor
    private void MoveCursor(Vector2 delta)
    {

    }

    // MENU: Click buttons
    private void ClickMenu()
    {

    }
}
