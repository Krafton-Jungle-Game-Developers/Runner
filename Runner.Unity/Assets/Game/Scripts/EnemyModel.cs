using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Runner.Game
{
    public class EnemyModel : MonoBehaviour
    {
        private SkinnedMeshRenderer _enemyRenderer;
        private Animator _animator;
        private EnemySoundController _soundController;
        public Transform childTransform;

        public BoolReactiveProperty IsVisible;
        public BoolReactiveProperty IsDead;
        private IObservable<EnemyModel> _onEnemyExecutableObservable;
        public IObservable<EnemyModel> OnEnemyExecutableObservable => _onEnemyExecutableObservable;
        
        private void Awake()
        {
            _enemyRenderer = GetComponent<SkinnedMeshRenderer>();
            _animator = GetComponent<Animator>();
            _soundController = GetComponent<EnemySoundController>();
            
            IsVisible = new(false);
            IsDead = new(false);
        }

        private void OnBecameVisible() => IsVisible.Value = true;
        private void OnBecameInvisible() => IsVisible.Value = false;

        public async UniTaskVoid Die()
        {
            IsDead.Value = true;
            _soundController.PlayDeathAudio();
            _animator.SetTrigger("DieTrigger");
            await UniTask.WhenAll(UniTask.WaitWhile(() => _soundController.EnemyAudioSource.isPlaying), 
                                  UniTask.WaitWhile(() => _animator.IsInTransition(0)));
            Destroy(gameObject);
        }
    }
}
