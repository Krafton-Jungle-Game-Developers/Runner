using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace Runner.Game
{
    public class PlayerEnemyPresenter : MonoBehaviour
    {
        private List<EnemyModel> _enemyModels;

        [Inject]
        private void Construct(List<EnemyModel> enemyModels)
        {
            _enemyModels = enemyModels;
        }
        
        private void Awake()
        {
            foreach (var enemyModel in _enemyModels)
            {
                enemyModel.OnEnemyBecameVisibleObservable.Subscribe(_ =>
                {
                    Debug.Log($"{enemyModel.gameObject.name} is Visible!");
                }).AddTo(this);

                enemyModel.OnEnemyBecameInvisibleObservable.Subscribe(_ =>
                {
                    Debug.Log($"{enemyModel.gameObject.name} is Invisible!");
                }).AddTo(this);
            }
        }
    }
}