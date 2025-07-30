using Zenject;


public interface IScene
{
    //Лучше вообще не использовать
    DiContainer DiContainer { get; }
    ObjectContainer Container { get; }
    IInteractor Interactor { get; }
}