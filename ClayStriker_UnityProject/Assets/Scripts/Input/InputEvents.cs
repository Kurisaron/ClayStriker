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

    private Action<Vector2> lookEvent;
    private Action fireEvent;

    //=================
    // UNITY FUNCTIONS
    //=================

    public override void Awake()
    {
        base.Awake();

        SetInputState(InputState.Game);
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

    //==============
    // INPUT STATES
    //==============

    // Used to switch input between playing the game and operating menus
    public void SetInputState(InputState state)
    {
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
