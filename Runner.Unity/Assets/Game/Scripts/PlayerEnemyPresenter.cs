using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Runner.Game
{
    public class PlayerEnemyPresenter : MonoBehaviour
    {
        [SerializeField] private List<EnemyModel> enemyModels = new();

        private void Awake()
        {
            foreach (var enemyModel in enemyModels)
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
