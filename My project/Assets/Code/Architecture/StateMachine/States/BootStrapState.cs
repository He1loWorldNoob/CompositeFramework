using Code.ApplicationServices.Client;
using Code.Infrastructure.StateMachine;

public class BootStrapState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly LoadCurtain _curtain;
    private readonly DiService _di;

    public BootStrapState(StateMachine stateMachine, SceneLoader sceneLoader, LoadCurtain curtain, DiService di)
    {
        _stateMachine = stateMachine;
        _sceneLoader = sceneLoader;
        _curtain = curtain;
        _di = di;
        RegisterServices();
    }

    private void RegisterServices()
    {

            
        _di.Bind<ObjectContainer>();
        _di.Bind<LogicTimer>();
        _di.Bind<Interactor, IInteractor>();
        _di.Bind<Scene>();

            
            
            
        //_di.Bind<UiFactory>();
    }

    public void Enter()
    {
        _sceneLoader.Load(ScenePath.InitScene, OnLoaded);
    }

        
    private void OnLoaded()
    {
            
    }
        
    public void Exit()
    {
    }
        
}