using Code.ApplicationServices.Client;
using Code.Infrastructure;
using Code.Infrastructure.StateMachine;
using Zenject;

public class Game
{
    private readonly StateMachine _stateMachine;
    public Game(ICourutineRunner courutineRunner, LoadCurtain loadCurtain)
    {
        var container = BuildGame(courutineRunner, loadCurtain);
        _stateMachine = container.Resolve<StateMachine>();
    }

    public void Start() => 
        _stateMachine.Enter<BootStrapState>();

    public void Update() => 
        _stateMachine.Update();


    public void ExitGame() => 
        _stateMachine.Enter<ExitGameState>();


    private DiContainer BuildGame(ICourutineRunner courutineRunner, LoadCurtain loadCurtain)
    {
            
        CMS.Init();
            
        DiService diService = DiService.Instance;
        DiContainer di = diService.Container;
            
        di.BindInstance(courutineRunner);
        di.BindInstance(loadCurtain);
        di.BindInstance(diService);
        di.Bind<SceneLoader>().AsSingle();
        di.Bind<StateFactory>().AsSingle();
        di.Bind<StateMachine>().AsSingle();
            
        return di;
    }
        
}