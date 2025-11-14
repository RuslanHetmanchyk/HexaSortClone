using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services.Scenes
{
    public class SceneLoaderService
    {
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private async UniTask LoadTargetSceneAsync(string sceneName)
        {
            await UniTask.DelayFrame(1); // чтобы дать сцене "LoadingScene" время отобразиться

            var asyncOp = SceneManager.LoadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = false;

            while (asyncOp.progress < 0.9f)
            {
                // Здесь можно обновлять UI прогресса
                Debug.Log($"Загрузка: {asyncOp.progress * 100}%");
                await UniTask.Yield();
            }

            // Прогрузка завершена
            Debug.Log("Готово к активации сцены");

            await UniTask.Delay(500); // Задержка, чтобы пользователь увидел "почти готово"
            asyncOp.allowSceneActivation = true;
        }
    }
}