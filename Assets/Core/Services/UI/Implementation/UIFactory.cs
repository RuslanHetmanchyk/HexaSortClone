using Core.Helpers;
using Core.Services.UI.Implementation.UIUnits;
using Core.Services.UI.Interfaces;
using UnityEngine;
using Zenject;

namespace Core.Services.UI.Implementation
{
    public class UIFactory : IUIFactory
    {
        private readonly DiContainer diContainer;

        public UIFactory(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public TScreen CreateScreen<TScreen>(Transform parent) where TScreen : UIScreen
        {
            var screenPrefab = Resources.Load<TScreen>($"UI/Screens/{typeof(TScreen).Name}");
            var instance = ContainerHelper
                .GetCurrentContainer(diContainer)
                .InstantiatePrefabForComponent<TScreen>(screenPrefab, parent);

            return instance;
        }

        public TPopup CreatePopup<TPopup>(Transform parent) where TPopup : UIPopup
        {
            var popupPrefab = Resources.Load<TPopup>($"UI/Popups/{typeof(TPopup).Name}");
            var instance = ContainerHelper
                .GetCurrentContainer(diContainer)
                .InstantiatePrefabForComponent<TPopup>(popupPrefab, parent);

            return instance;
        }
    }
}