using Code.ApplicationServices.Client;
using Code.Infrastructure.StateMachine;
using UnityEngine;

public class ExitGameState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly LoadCurtain _curtain;

    public ExitGameState(StateMachine stateMachine, SceneLoader sceneLoader, LoadCurtain curtain)
    {
        _stateMachine = stateMachine;
        _sceneLoader = sceneLoader;
        _curtain = curtain;
    }

    public void Enter()
    {
        Debug.Log("ExitGame");
    }
        
    public void Exit()
    {
    }
        
}