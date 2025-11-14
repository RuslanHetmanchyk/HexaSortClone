using UnityEngine;
using Zenject;

namespace Core.Helpers
{
    public static class ContainerHelper
    {
        public static DiContainer GetCurrentContainer(DiContainer fallback)
        {
            var sceneContext = Object.FindObjectOfType<SceneContext>();
            return sceneContext != null ? sceneContext.Container : fallback;
        }
    }
}