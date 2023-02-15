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
        private Animator _animator;
        private int _dieTrigger = Animator.StringToHash("Die");

        public BoolReactiveProperty IsVisible;
        private bool _isDead = false;
        public bool IsDead => _isDead;
        private IObservable<EnemyModel> _onEnemyExecutableObservable;
        public IObservable<EnemyModel> OnEnemyExecutableObservable => _onEnemyExecutableObservable;
        
        private void Awake()
        {
            _enemyRenderer = GetComponent<SkinnedMeshRenderer>();
            _animator = GetComponent<Animator>();
            IsVisible = new(false);
        }

        private void OnBecameVisible() => IsVisible.Value = true;
        private void OnBecameInvisible() => IsVisible.Value = false;

        private void Die()
        {
            _animator.SetTrigger(_dieTrigger);
        }
    }
}
