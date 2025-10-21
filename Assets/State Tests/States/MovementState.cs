using Shears;
using Shears.Input;
using Shears.StateMachineGraphs;
using System;
using UnityEngine;

[StateMenuItem("Example States/Movement")]
public class MovementState : State<ManagedInputMap, CharacterController, Transform>
{
    private const float MOVE_SPEED = 4.0f;

    private CharacterController controller;
    private Transform relativeTransform;
    private IManagedInput moveInput;

    protected override void Inject(ManagedInputMap inputMap, CharacterController dependency, Transform relativeTransform)
    {
        controller = dependency;
        moveInput = inputMap.GetInput("Move");
        this.relativeTransform = relativeTransform;
    }

    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
    }

    protected override void OnUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 input = moveInput.ReadValue<Vector2>();
        input = input.normalized;

        Vector3 forward = relativeTransform.forward.With(y: 0).normalized;
        Vector3 right = relativeTransform.right.With(y: 0).normalized;

        Vector3 forwardMovement = input.y * MOVE_SPEED * forward;
        Vector3 rightMovement = input.x * MOVE_SPEED * right;
        Vector3 movement = forwardMovement + rightMovement;

        controller.Move(movement * Time.deltaTime);
    }
}
