using System.Collections.Generic;
using Zenject;

public class DiService
{
    private static DiService _instance;
    public static DiService Instance => _instance ?? (_instance = new DiService());

    public DiContainer Container = new();
    public DiContainer LocalDi;

    public void WarmUp()
    {
        LocalDi = new DiContainer(Container);
    }

    public T Bind<T>()
    {
        Container.Bind<T>().AsSingle();
        var instance = Container.Resolve<T>();
        return instance;
    }
    public TInterface Bind<T, TInterface>() where T : TInterface
    {
        Container.Bind<TInterface>().To<T>().AsSingle();
        var instance = Container.Resolve<TInterface>();
        return instance;
    }
}