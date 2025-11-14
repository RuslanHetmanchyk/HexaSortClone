using UnityEngine;

namespace Core.Config.AssetsTypes
{
    [CreateAssetMenu(
        menuName = "Config/HexGrid/" + nameof(GridConfigData),
        fileName = nameof(GridConfigData))]
    public class GridConfigData : ScriptableObject
    {
        [SerializeField] private int width = 11;
        [SerializeField] private int height = 9;

        public int Width => width;
        public int Height => height;
    }
}