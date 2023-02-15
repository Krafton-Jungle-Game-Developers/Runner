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
        [SerializeField] private float fallThroughCheckRayOriginYOffset = 1f;
        [SerializeField] private float fallThroughCheckRayDistance = 0.5f;
        [SerializeField] private LayerMask groundLayer;

        public BoolReactiveProperty CanExecute;

        private PlayerCameraController _cameraController;
        private CapsuleCollider _collider;
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
            _collider = GetComponent<CapsuleCollider>();
            _movementController = GetComponent<PlayerMovementController>();
            _inputController = GetComponent<PlayerInputController>();
            groundLayer = LayerMask.NameToLayer("Ground");
            CanExecute = new(true);

            _inputController.OnExecuteInputObservable
            .Where(_ => CanExecute.Value)
            .Select(_ =>
            {
                return _enemyModels.Where(enemy => enemy is not null
                                                && enemy.IsDead.Value is false
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
            _cameraController.freezeMouse = true;
            _movementController.canControl = false;
            await transform.DOMove(targetPosition, approachTime).SetEase(executeApproachEase);

            Vector3 rayOrigin = new(transform.position.x, transform.position.y + fallThroughCheckRayOriginYOffset, transform.position.z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, fallThroughCheckRayDistance, groundLayer))
            {
                Debug.Log("hit");
                transform.position = new(transform.position.x, hit.point.y + 0.5f, transform.position.z);
            }
            _cameraController.freezeMouse = false;
            _movementController.canControl = true;
            transform.LookAt(enemy.transform.position);

            await enemy.Die();
        }
    }
}
