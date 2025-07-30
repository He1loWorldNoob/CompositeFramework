using Code.ApplicationServices.Client;
using Code.Infrastructure.StateMachine;

public class TestState : IState, IUpdateble
{
    private StateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private LoadCurtain _loadCurtain;
    private readonly Scene _scene;
    
    public TestState(StateMachine stateMachine, LoadCurtain loadCurtain, Scene scene, SceneLoader sceneLoader)
    {
        _stateMachine = stateMachine;
        _loadCurtain = loadCurtain;
        _scene = scene;
        _sceneLoader = sceneLoader;
    }

    public void Exit()
    {
        _loadCurtain.Show();
        _scene.WarmUp();
        _sceneLoader.Load("TestScene", OnLoaded);
    }

    private void OnLoaded()
    {
        
        _scene.Bind<InitializeUnitSystem>();
        _scene.Bind<MovementSystem>();
        _scene.Bind<ObjectViewLifeCycle>();
        
        _scene.Initialize();
        _loadCurtain.Hide();
    }
    
    public void OnUpdate() => 
        _scene.Update();

    public void Enter() => 
        _scene.Cleanup();
}