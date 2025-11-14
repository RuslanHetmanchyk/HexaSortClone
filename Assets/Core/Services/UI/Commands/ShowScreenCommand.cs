using Core.Services.CommandRunner.Interfaces.Command;
using Core.Services.UI.Implementation.UIUnits;
using Core.Services.UI.Interfaces;
using Zenject;

namespace Core.Services.UI.Commands
{
    public class ShowScreenCommand<TScreen> : ICommand
        where TScreen : UIScreen
    {
        private IUIService uiService;

        [Inject]
        public void Construct(IUIService uiService)
        {
            this.uiService = uiService;
        }

        public void Execute()
        {
            uiService.ShowScreen<TScreen>();
        }
    }
}