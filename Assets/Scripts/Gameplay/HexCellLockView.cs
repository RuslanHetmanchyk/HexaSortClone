using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class HexCellLockView : MonoBehaviour
    {
        [SerializeField] private GameObject adsLockObject;
        [SerializeField] private GameObject coinsLockObject;
        
        private Dictionary<LockType, GameObject> lockObjects;

        private void Awake()
        {
            lockObjects = new Dictionary<LockType, GameObject>
            {
                { LockType.WatchAds, adsLockObject },
                { LockType.PayCoins, coinsLockObject }
            };
        }

        public void EnableLock(LockType lockType, int lockValue)
        {
            lockObjects[lockType].SetActive(true);
        }

        public void DisableLock()
        {
            foreach (var item in lockObjects.Values)
            {
                item.SetActive(false);
            }
        }
    }
}