using Code.ApplicationServices.Client;
using Code.Infrastructure;
using UnityEngine;

public class GameBootstraper : MonoBehaviour, ICourutineRunner
{
    [SerializeField] private LoadCurtain loadCurtain;
    private Game _game;

    private void Awake()
    {
        _game = new Game(this, Instantiate(loadCurtain,transform));
        _game.Start();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        _game.Update();
    }

    private void OnApplicationQuit()
    {
        _game.ExitGame();
    }
}

