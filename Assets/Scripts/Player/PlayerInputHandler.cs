using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : PlayerComponentsGetter
{
    protected event EventHandler<float> MovementPerformed;
    protected event EventHandler MovementCanceled;

    protected event EventHandler JumpPerformed;
    protected event EventHandler JumpCanceled;

    protected event EventHandler AttackPerformed;
    protected event EventHandler AttackCanceled;

    protected event EventHandler SlowMotionPerformed;
    protected event EventHandler SlowMotionCanceled;

    protected event EventHandler PossessPerformed;
    protected event EventHandler PossessCanceled;

    protected event EventHandler ZoomInPerformed;
    protected event EventHandler ZoomOutCanceled;

    protected event EventHandler RestartPerformed;

    protected event EventHandler PickUpThrowPerformed;

    protected event EventHandler CrouchPerformed;

    protected event EventHandler ExplodePerformed;
    protected event EventHandler ExplodeCanceled;

    private PlayerControls playerControls;

    protected new void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();

        playerControls.Player.Movement.performed += Movement_performed;
        playerControls.Player.Movement.canceled += Movement_canceled;

        playerControls.Player.Jump.performed += Jump_performed;
        playerControls.Player.Jump.canceled += Jump_canceled;

        playerControls.Player.Attack.performed += Attack_performed;
        playerControls.Player.Attack.canceled += Attack_canceled;

        playerControls.Player.SlowMotion.performed += SlowoMotion_performed;
        playerControls.Player.SlowMotion.canceled += SlowMotion_canceled;

        playerControls.Player.Possess.performed += Possess_performed;
        playerControls.Player.Possess.canceled += Possess_canceled;

        playerControls.Player.ZoomInOut.performed += ZoomInOut_performed;
        playerControls.Player.ZoomInOut.canceled += ZoomInOut_canceled;

        playerControls.Player.Restart.performed += Restart_performed;

        playerControls.Player.PickUp.performed += PickUp_performed;

        playerControls.Player.Crouch.performed += Crouch_performed;

        playerControls.Player.Explode.performed += Explode_performed;
        playerControls.Player.Explode.canceled += Explode_canceled;
    }

    private void ZoomInOut_performed(InputAction.CallbackContext obj)
    {
        ZoomInPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void ZoomInOut_canceled(InputAction.CallbackContext obj)
    {
        ZoomOutCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_performed(InputAction.CallbackContext obj)
    {
        CrouchPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void PickUp_performed(InputAction.CallbackContext obj)
    {
        PickUpThrowPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Restart_performed(InputAction.CallbackContext obj)
    {
        RestartPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        MovementPerformed?.Invoke(this, obj.ReadValue<float>());
    }

    private void Movement_canceled(InputAction.CallbackContext obj)
    {
        MovementCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        JumpPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        JumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        AttackPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_canceled(InputAction.CallbackContext obj)
    {
        AttackCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void SlowoMotion_performed(InputAction.CallbackContext obj)
    {
        SlowMotionPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void SlowMotion_canceled(InputAction.CallbackContext obj)
    {
        SlowMotionCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Possess_canceled(InputAction.CallbackContext obj)
    {
        PossessCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Possess_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log($"Possess Performed: {Time.time}");

        PossessPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Explode_performed(InputAction.CallbackContext obj)
    {
        ExplodePerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Explode_canceled(InputAction.CallbackContext obj)
    {
        ExplodeCanceled?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnEnable()
    {
        playerControls.Enable();
    }

    protected virtual void OnDisable()
    {
        playerControls.Disable();
    }
}
