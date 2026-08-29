using System;
using UnityEngine;

namespace DesertDash.World
{
    public abstract class PooledPickup : MonoBehaviour
    {
        private Action<GameObject> _release;
        private bool _available;

        public virtual void Activate(Action<GameObject> release)
        {
            _release = release;
            _available = true;
        }

        protected bool Consume()
        {
            if (!_available)
            {
                return false;
            }

            _available = false;
            _release?.Invoke(gameObject);
            return true;
        }
    }
}
