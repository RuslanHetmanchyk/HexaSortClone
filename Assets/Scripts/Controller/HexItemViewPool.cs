using Core.Services.Gameplay.Level.Implementation;
using Gameplay;
using Zenject;

namespace Controller
{
    public class HexItemViewPool : MonoMemoryPool<HexItem, HexItemView>
    {
        protected override void Reinitialize(HexItem model, HexItemView view)
        {
            view.Init(model);
        }
    }
}