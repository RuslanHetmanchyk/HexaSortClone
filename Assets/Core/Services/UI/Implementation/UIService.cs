using System;
using System.Collections.Generic;
using Core.Services.UI.Implementation.UIUnits;
using Core.Services.UI.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Services.UI.Implementation
{
    public class UIService : IUIService, IDisposable
    {
        private const string UI_ROOT_PREFAB_PATH = "UIRoot";

        private readonly Dictionary<Type, UIPopup> createdPopups = new();
        private readonly Dictionary<Type, UIScreen> createdScreens = new();

        private readonly IUIFactory uiFactory;

        private UIScreen activeScreen;

        public UIRoot UIRoot { get; }

        public UIService(IUIFactory uiFactory)
        {
            this.uiFactory = uiFactory;

            // Create UIRoot
            var rootCanvasPrefab = Resources.Load<UIRoot>(UI_ROOT_PREFAB_PATH);
            UIRoot = Object.Instantiate(rootCanvasPrefab);
            Object.DontDestroyOnLoad(UIRoot);
        }

        public void Dispose()
        {
            if (UIRoot)
            {
                Object.Destroy(UIRoot.gameObject);
            }
        }

        public TScreen GetScreen<TScreen>() where TScreen : UIScreen
        {
            if (!createdScreens.ContainsKey(typeof(TScreen)))
            {
                createdScreens.Add(typeof(TScreen), uiFactory.CreateScreen<TScreen>(UIRoot.transform));
            }

            return createdScreens[typeof(TScreen)] as TScreen;
        }

        public TScreen ShowScreen<TScreen>() where TScreen : UIScreen
        {
            var screenType = typeof(TScreen);
            if (activeScreen != null && activeScreen.GetType() == screenType)
            {
                return activeScreen as TScreen;
            }

            if (activeScreen != null)
            {
                activeScreen.Hide();
            }

            activeScreen = GetScreen<TScreen>();
            activeScreen.Show();

            return activeScreen as TScreen;
        }

        public void HideScreen<TScreen>() where TScreen : UIScreen
        {
            GetScreen<TScreen>().Hide();
        }

        public TPopup GetPopup<TPopup>() where TPopup : UIPopup
        {
            if (!createdPopups.ContainsKey(typeof(TPopup)))
            {
                createdPopups.Add(typeof(TPopup), uiFactory.CreatePopup<TPopup>(UIRoot.transform));
            }

            return createdPopups[typeof(TPopup)] as TPopup;
        }

        public TPopup ShowPopup<TPopup>() where TPopup : UIPopup
        {
            var popup = GetPopup<TPopup>();
            popup.Show();

            return popup;
        }

        public void HidePopup<TPopup>() where TPopup : UIPopup
        {
            GetPopup<TPopup>().Hide();
        }
    }
}