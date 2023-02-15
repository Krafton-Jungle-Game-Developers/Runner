using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace Runner.Game
{
    public class PlayerAbilityController : MonoBehaviour
    {
        [SerializeField] private float executeDistance = 15f;
        private BoolReactiveProperty _canExecute;
        public IReactiveProperty<bool> CanExecute => _canExecute;

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

            _canExecute = new(true);

            IObservable<Vector3> merged = Observable.Merge(_enemyModels.Select(x => x.OnEnemyBecameVisibleObservable
                                                    .Where(enemyPosition => Vector3.Distance(enemyPosition, transform.position) <= executeDistance)));
            Observable.ZipLatest(_inputController.OnExecuteInputObservable, merged)
                      .Subscribe(_ =>
                      {
                          Execute(_.Last());
                      })
                      .AddTo(this);
        }

        private void Execute(Vector3 enemyPosition)
        {
            Vector3 direction = enemyPosition - transform.position;
            transform.position = enemyPosition - direction.normalized * 2f;
            
            transform.LookAt(enemyPosition);
        }
    }
}
