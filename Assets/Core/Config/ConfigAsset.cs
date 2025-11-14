using System.Collections.Generic;
using Core.Config.AssetsTypes;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(
        menuName = "Config/" + nameof(ConfigAsset),
        fileName = nameof(ConfigAsset))]
    public class ConfigAsset : ScriptableObject
    {
        [SerializeField] private GridConfigData gridConfig;

        [SerializeField] private List<HexLevel> levels;

        public GridConfigData GridConfig => gridConfig;

        public HexLevel GetLevel(int levelIndex)
        {
            return levels[levelIndex];
        }
    }
}