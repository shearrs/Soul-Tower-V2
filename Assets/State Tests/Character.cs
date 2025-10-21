using UnityEngine;
using Shears.StateMachineGraphs;
using Shears.Input;

public class Character : MonoBehaviour
{
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private ManagedInputMap inputMap;

    private IManagedInput moveInput;

    private void Awake()
    {
        moveInput = inputMap.GetInput("Move");
    }

    private void Update()
    {
        
    }

    private void UpdateMovement(Vector2 input)
    {

    }
}
