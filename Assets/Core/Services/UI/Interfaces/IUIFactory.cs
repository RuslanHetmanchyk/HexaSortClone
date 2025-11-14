using Core.Services.UI.Implementation.UIUnits;
using UnityEngine;

namespace Core.Services.UI.Interfaces
{
    public interface IUIFactory
    {
        TScreen CreateScreen<TScreen>(Transform parent) where TScreen : UIScreen;
        TPopup CreatePopup<TPopup>(Transform parent) where TPopup : UIPopup;
    }
}