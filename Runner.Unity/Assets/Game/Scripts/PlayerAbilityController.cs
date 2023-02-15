using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using UniRx;
using DG.Tweening;

namespace Runner.Game
{
    public class PlayerAbilityController : MonoBehaviour
    {
        [SerializeField] private Ease executeApproachEase = Ease.InExpo;
        [SerializeField] private float approachTime = 0.3f;
        [SerializeField] private float executeThrottleTime = 0.5f;
        [SerializeField] private float executeDistance = 15f;
        [SerializeField] private float approachDistance = 2f;
        
        public BoolReactiveProperty CanExecute;

        private PlayerCameraController _cameraController;
        private PlayerMovementController _movementController;
        private PlayerInputController _inputController;
        private List<EnemyModel> _enemyModels;

        [Inject]
        private void Construct(List<EnemyModel> enemyModels)
        {
            _enemyModels = enemyModels;
        }

        private void Start()
        {
            _cameraController = GetComponentInChildren<PlayerCameraController>();
            _movementController = GetComponent<PlayerMovementController>();
            _inputController = GetComponent<PlayerInputController>();

            CanExecute = new(true);

            _inputController.OnExecuteInputObservable
            .Where(_ => CanExecute.Value)
            .Select(_ =>
            {
                return _enemyModels.Where(enemy => enemy is not null
                                                && enemy.IsDead is false
                                                && enemy.IsVisible.Value
                                                && Vector3.Distance(enemy.transform.position, transform.position) <= executeDistance)
                                   .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
                                   .FirstOrDefault();
            })
            .Subscribe(async (enemy) =>
            {
                CanExecute.Value = false;
                if (enemy is not null) { await Execute(enemy, this.GetCancellationTokenOnDestroy()); }
                CanExecute.Value = true;
            }).AddTo(this);
        }

        private async UniTask Execute(EnemyModel enemy, CancellationToken token)
        {
            Vector3 direction = enemy.transform.position - transform.position;
            Vector3 targetPosition = enemy.transform.position - direction.normalized;
            transform.LookAt(enemy.transform.position);
            await transform.DOMove(targetPosition, approachTime).SetEase(executeApproachEase);
            _cameraController.freezeMouse = true;
            _movementController.canControl = false;
            float currentDistance = float.MaxValue;
            while (Vector3.Distance(transform.position, targetPosition) > float.Epsilon)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * approachTime);
                await UniTask.Yield();
            }
            _cameraController.freezeMouse = false;
            _movementController.canControl = true;
            Destroy(enemy.gameObject);
        }
    }
}
