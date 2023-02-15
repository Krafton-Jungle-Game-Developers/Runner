using System.Collections.Generic;
using Runner.Game;
using TMPro;
using UnityEngine;
using Zenject;

namespace Runner.UI
{
    // Canvas
    public class HUDPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text slayCountText;
        [SerializeField] private TMP_Text totalEnemyText;
        private List<EnemyModel> _enemyModels;

        [Inject]
        private void Construct(List<EnemyModel> enemyModels)
        {
            _enemyModels = enemyModels;
        }

        private void Awake()
        {
            slayCountText.text = "0";
            totalEnemyText.text = $"{_enemyModels.Count}";
        }
    }
}