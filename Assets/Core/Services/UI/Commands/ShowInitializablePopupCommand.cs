using Core.Services.CommandRunner.Interfaces.Command;
using Core.Services.UI.Implementation.UIUnits;
using Core.Services.UI.Interfaces;
using Zenject;

namespace Core.Services.UI.Commands
{
    public class ShowInitializablePopupCommand<TPopup, TCommandData> : ICommandWithData<TCommandData>
        where TPopup : UIPopup, IInitializableUIUnit<TCommandData>
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
            uiService.ShowPopup<TPopup>().Init(commandData);
        }
    }
}