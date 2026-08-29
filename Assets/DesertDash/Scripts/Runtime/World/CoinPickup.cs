using DesertDash.Core;
using UnityEngine;

namespace DesertDash.World
{
    public sealed class CoinPickup : PooledPickup
    {
        private Transform _attractionTarget;
        private GameManager _game;

        public override void Activate(System.Action<GameObject> release)
        {
            base.Activate(release);
            _attractionTarget = null;
            _game = null;
        }

        private void Update()
        {
            if (_attractionTarget == null || _game == null || _game.State != GameState.Running)
            {
                return;
            }

            var destination = _attractionTarget.position + Vector3.up * 1.1f;
            var distance = Vector3.Distance(transform.position, destination);
            transform.position = Vector3.MoveTowards(transform.position, destination, (12f + distance * 3f) * Time.deltaTime);
            if (distance < 0.65f)
            {
                TryCollect(_game);
            }
        }

        public void BeginAttraction(Transform target, GameManager game)
        {
            if (_attractionTarget == null)
            {
                _attractionTarget = target;
                _game = game;
            }
        }

        public bool TryCollect(GameManager game)
        {
            if (!Consume())
            {
                return false;
            }

            game.CollectCoin();
            return true;
        }
    }
}
