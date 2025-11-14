using UnityEngine;

namespace Core.Services.UI.Implementation.UIUnits
{
    public class BaseUIUnit : MonoBehaviour
    {
        public bool IsShowing => gameObject.activeSelf;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}