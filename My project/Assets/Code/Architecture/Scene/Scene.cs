using System.Linq;
using Zenject;

public class Scene : IScene
{
    public IInteractor Interactor => _interactor;
    public DiContainer DiContainer => _di.LocalDi;
    public ObjectContainer Container => _container;
    private readonly DiService _di;
    private readonly LogicTimer _logicTimer;
    private readonly ObjectContainer _container;
    private readonly IInteractor _interactor;
    public Scene(IInteractor interactor, ObjectContainer container, DiService di, LogicTimer logicTimer)
    {
        _interactor = interactor;
        _container = container;
        _di = di;
        _logicTimer = logicTimer;
    }
    

    public void WarmUp() => 
        _di.WarmUp();

    

    public Scene BindModule<T>() where T : IDiModule
    {
        var module = DiContainer.Instantiate<T>();
        module.BindModule(this);
        return this;
    }
    
    public Scene Bind<T>()
    {
        _di.LocalDi.Bind<T>().AsSingle().NonLazy();
        var instance = DiContainer.Resolve<T>();
        RegisterInteraction(instance);
        return this;
    }
    public Scene Bind<T>(params object[] parameters)
    {
        var instance = _di.LocalDi.Instantiate<T>(parameters);
        BindInstance(instance);
        return this;
    }

    private void RegisterInteraction<T>(T instance)
    {
        if (instance is BaseInteraction interaction) 
            _interactor.BindInteraction(interaction);
    }

    public Scene BindInstance<T>(T instance)
    {
        DiContainer.BindInstance(instance);
        RegisterInteraction(instance);
        return this;
    }

    public bool IsStarted { get; private set; }

    public void Initialize()
    {
        _interactor.CallAll<IInitializable>(x => x.OnInitialize());
        IsStarted = true;
    }

    public void Update()
    {
        if(!IsStarted) return;
        _interactor.CallAll<IUpdateble>(x => x.OnUpdate());
        _logicTimer.TimerTick(OnLogicUpdate);
    }


    public void OnLogicUpdate(float dt)
    {
        ProcessDestroyObjects();
        ProcessUpdateObjects(dt);
        ProcessAwakeObjects();
        _interactor.CallAll<ILogicUpdateble>(x => x.OnLogicUpdate(dt));

    }

    private void ProcessDestroyObjects()
    {
        foreach (var state in _container.Objects.ToList())
        {
            if (!state.Is<Destroy>()) continue;
            _container.Remove(state.Id);
            _interactor.CallAll<IDestroyObjectModule>(x=>x.OnDestroyObject(state));
        }
    }

    private void ProcessAwakeObjects()
    {
        foreach (var state in _container.Objects.ToList())
        {
            if (!state.Is<Awake>()) continue;
            _interactor.CallAll<IAwakeObjectModule>(x=>x.OnAwakeObject(state));
            state.Remove<Awake>();
            _interactor.CallAll<IStartObjectModule>(x=>x.OnStartObject(state));
        }
    }

    private void ProcessUpdateObjects(float dt)
    {
        foreach (var state in _container.Objects.ToList())
        {
            _interactor.CallAll<ILogicUpdateObjectModule>(x=>x.OnLogicUpdateObject(state, dt));
        }
    }
    
    

    public void Cleanup()
    {
        IsStarted = false;
        
        _interactor.CallAll<ICleanup>(x => x.OnCleanup());
        foreach (var state in _container.Objects) 
            _interactor.CallAll<IDestroyObjectModule>(x=>x.OnDestroyObject(state));
        
        _container.Clear();
        _interactor.Clear();

    }

}