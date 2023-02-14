using System.Collections.Generic;
using System.Linq;
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

        //private void Awake()
        //{
        //    _movementController = GetComponent<PlayerMovementController>();
        //    _inputController = GetComponent<PlayerInputController>();

        //    _canExecute = new(true);
            
        //    var merged = Observable.Merge(_enemyModels.Select(x => x.OnBecameVisibleObservable)
        //                                            .Take(1));
        //    Observable.ZipLatest(_inputController.OnExecuteInputObservable, merged)
        //              .Subscribe(_ => _);
        //}
    }
}
