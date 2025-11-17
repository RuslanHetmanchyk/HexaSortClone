using Core.Services.UI.Implementation.UIUnits;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Popups
{
    public class NotificationPopup : UIPopup
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private CanvasGroup canvasGroup;

        public override void Show()
        {
            base.Show();

            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0f, 0.5f)
                .SetDelay(1)
                .SetEase(Ease.Linear)
                .OnComplete(Hide);
        }

        public void SetLabel(string text)
        {
            label.text = text;
        }
    }
}