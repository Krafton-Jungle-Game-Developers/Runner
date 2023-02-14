using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Runner.UI;
using Zenject;

namespace Runner.Game
{
    public class EnemyModel : MonoBehaviour
    {
        private PlayerEnemyPresenter _player;
        private HUDPresenter _HUD;

        [Inject]
        private void Construct(PlayerEnemyPresenter player, HUDPresenter HUD)
        {
            _player = player;
            _HUD = HUD;
        }

        private Subject<Vector3> _onBecameVisible;
        public IObservable<Vector3> OnBecameVisibleObservable => _onBecameVisible;

        private ReactiveProperty<bool> _isDeadProperty;
        public IReactiveProperty<bool> IsDeadProperty => _isDeadProperty;

        private void Awake()
        {
            _onBecameVisible = new();
            _isDeadProperty = new();
        }

        public void OnBecameVisible() => _onBecameVisible.OnNext(transform.position);
    }
}
