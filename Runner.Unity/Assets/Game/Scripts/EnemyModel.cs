using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Runner.Game
{
    public class EnemyModel : MonoBehaviour
    {
        private IObservable<Unit> _onEnemyBecameVisibleObservable;
        private IObservable<Unit> _onEnemyBecameInvisibleObservable;
        public IObservable<Unit> OnEnemyBecameVisibleObservable => _onEnemyBecameVisibleObservable;
        public IObservable<Unit> OnEnemyBecameInvisibleObservable => _onEnemyBecameInvisibleObservable;

        private void Awake()
        {
            _onEnemyBecameVisibleObservable = this.OnBecameVisibleAsObservable();
            _onEnemyBecameInvisibleObservable = this.OnBecameInvisibleAsObservable();
        }
    }
}
