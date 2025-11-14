using Core.Services.CommandRunner.Interfaces.Command;
using Core.Services.UI.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Services.Scenes.Commands
{
    public class LoadLevelCommand<TCommandData> : ICommandWithData<TCommandData>
        where TCommandData : struct
    {
        private IUIService uiService;

        [Inject]
        public void Construct(IUIService uiService)
        {
            this.uiService = uiService;
        }

        public void Execute(TCommandData commandData)
        {
            var data = (LoadLevelData)(object)commandData;
            LoadSceneAsync(data.SceneName).Forget();
        }
        
        private async UniTask LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
        }
    }

    public struct LoadLevelData
    {
        public string SceneName;
    }
}