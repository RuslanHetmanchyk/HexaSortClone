using Core.Services.Gameplay.Level.Implementation;
using Gameplay;
using Zenject;

namespace Controller
{
    public class HexStackViewPool : MonoMemoryPool<HexStack, HexStackView>
    {
        protected override void Reinitialize(HexStack model, HexStackView view)
        {
            view.Init(model);
        }
    }
}