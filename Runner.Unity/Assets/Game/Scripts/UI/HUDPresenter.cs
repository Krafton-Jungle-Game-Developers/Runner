using System.Linq;
using System.Collections.Generic;
using Runner.Game;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Runner.UI
{
    // Canvas
    public class HUDPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text slayCountText;
        [SerializeField] private TMP_Text totalEnemyText;
        public IntReactiveProperty SlayCount;
        private List<EnemyModel> _enemyModels;

        [Inject]
        private void Construct(List<EnemyModel> enemyModels)
        {
            _enemyModels = enemyModels;
        }

        private void Start()
        {
            SlayCount = new(0);
            slayCountText.text = $"{SlayCount}";
            totalEnemyText.text = $"{_enemyModels.Count}";

            Observable.Merge(_enemyModels.Select(_ => _.IsDead)).Subscribe(_ =>
            {
                SlayCount.Value = Mathf.Min(SlayCount.Value + 1, _enemyModels.Count);
                slayCountText.text = $"{SlayCount.Value}";
            }).AddTo(this);
        }
    }
}