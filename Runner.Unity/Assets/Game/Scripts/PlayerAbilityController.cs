using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Runner.Game
{
    public class PlayerAbilityController : MonoBehaviour
    {
        [SerializeField] private float executeThrottleTime = 0.5f;
        [SerializeField] private float executeDistance = 15f;
        [SerializeField] private float approachDistance = 2f;
        
        public BoolReactiveProperty CanExecute;

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
            .Subscribe(enemy =>
            {
                CanExecute.Value = false;
                if (enemy is not null) { await Execute(enemy); }
                CanExecute.Value = true;
            });
        }

        private async UniTaskVoid Execute(EnemyModel enemy)
        {
            Vector3 direction = enemy.transform.position - transform.position;
            transform.position = enemy.transform.position - direction.normalized * approachDistance;
            transform.LookAt(enemy.transform.position);
            Destroy(enemy.gameObject);
        }
    }
}
