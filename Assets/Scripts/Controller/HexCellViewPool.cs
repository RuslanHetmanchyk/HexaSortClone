using Core.Services.Gameplay.Level.Implementation;
using Gameplay;
using Zenject;

namespace Controller
{
    public class HexCellViewPool : MonoMemoryPool<HexCell, HexCellView>
    {
        protected override void Reinitialize(HexCell model, HexCellView view)
        {
            view.Init(model);
        }
    }
}