using Shears.Input;
using Shears.StateMachineGraphs;
using System;
using UnityEngine;

[StateMenuItem("Example States/Locomotion")]
public class NavigationState : State<ManagedInputMap>
{
    [SerializeField] private int value;

    private IManagedInput moveInput;
    private Guid moveInputID;

    protected override void Inject(ManagedInputMap dependency)
    {
        moveInput = dependency.GetInput("Move");
        moveInputID = GetParameterID("moveInput");
    }

    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
    }

    protected override void OnUpdate()
    {
        float inputMagnitude = moveInput.ReadValue<Vector2>().sqrMagnitude;

        SetParameter(moveInputID, inputMagnitude);
    }
}
