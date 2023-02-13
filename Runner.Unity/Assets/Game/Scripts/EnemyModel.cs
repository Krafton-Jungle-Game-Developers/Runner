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
        [SerializeField] private GameObject indicatorPrefab;
        private PlayerEnemyPresenter _player;
        private HUDPresenter _HUD;

        [Inject]
        private void Construct(PlayerEnemyPresenter player, HUDPresenter HUD)
        {
            _player = player;
            _HUD = HUD;
        }
        
        private IObservable<Unit> _onEnemyBecameVisibleObservable;
        private IObservable<Unit> _onEnemyBecameInvisibleObservable;
        public IObservable<Unit> OnEnemyBecameVisibleObservable => _onEnemyBecameVisibleObservable;
        public IObservable<Unit> OnEnemyBecameInvisibleObservable => _onEnemyBecameInvisibleObservable;

        private void Awake()
        {
            _onEnemyBecameVisibleObservable = this.OnBecameVisibleAsObservable();
            _onEnemyBecameInvisibleObservable = this.OnBecameInvisibleAsObservable();
        }

        private void Start()
        {
            _HUD.CreateIndicator(this);
        }
    }
}
