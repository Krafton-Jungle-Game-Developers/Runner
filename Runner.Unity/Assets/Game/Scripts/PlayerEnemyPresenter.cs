using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;

namespace Runner.Game
{
    public class PlayerEnemyPresenter : MonoBehaviour
    {
        [SerializeField] private float executeDistance = 15f;

        private BoolReactiveProperty _canExecute;
        public IReactiveProperty<bool> CanExecute => _canExecute;

        private PlayerMovementController _playerController;
        private List<EnemyModel> _enemyModels;

        [Inject]
        private void Construct(List<EnemyModel> enemyModels)
        {
            _enemyModels = enemyModels;
        }
    }
}