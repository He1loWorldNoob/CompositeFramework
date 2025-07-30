using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.ApplicationServices.Client
{
    public class SceneLoader
    {
        public void Load(string sceneName, Action onLoaded = null)
        {
            LoadTask(sceneName, onLoaded).Forget();
        }
        
        
        private async UniTask LoadTask(string sceneName, Action onLoaded = null)
        {
            Debug.Log($"{sceneName} SceneLoad");

            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                return;
            }

            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
            onLoaded?.Invoke();
        }
    }
    
    public static class ScenePath
    {
        
        public const string InitScene = "Init";
        public const string HubScene = "Hub";
        public const string MapScene = "Map";
        public const string MainMenu = "MainMenu";
        public const string Battle = "Battle";
    }
}
