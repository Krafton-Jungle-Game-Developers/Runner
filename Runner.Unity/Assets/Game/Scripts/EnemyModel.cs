using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Runner.UI;
using Zenject;

namespace Runner.Game
{
    public class EnemyModel : MonoBehaviour
    {
        private SkinnedMeshRenderer _enemyRenderer;
        private PlayerAbilityController _player;
        private HUDPresenter _HUD;

        [Inject]
        private void Construct(PlayerAbilityController player, HUDPresenter HUD)
        {
            _player = player;
            _HUD = HUD;
        }
        
        private ReactiveProperty<bool> _isDead;
        public IReactiveProperty<bool> IsDead => _isDead;
        private IObservable<Vector3> _onEnemyBecameVisibleObservable;
        public IObservable<Vector3> OnEnemyBecameVisibleObservable => _onEnemyBecameVisibleObservable;
        
        private void Awake()
        {
            _enemyRenderer = GetComponent<SkinnedMeshRenderer>();
            _onEnemyBecameVisibleObservable = Observable.Interval(TimeSpan.FromSeconds(0.1f))
                                                        .TakeWhile(_ => _enemyRenderer.isVisible)
                                                        .Repeat()
                                                        .Select(_ => transform.position);
            _isDead = new(false);
        }
    }
}
