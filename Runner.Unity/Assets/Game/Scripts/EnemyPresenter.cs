using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runner.UI
{
    public class EnemyPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject indicatorPrefab;
        private HUDPresenter _HUD;
        private HUDIndicator _indicator;

        [Inject]
        private void Construct(HUDPresenter HUD)
        {
            _HUD = HUD;
        }

        private void Awake()
        {
            GameObject indicator = Instantiate(indicatorPrefab,
                                               Vector3.zero,
                                               Quaternion.identity,
                                               _HUD.elementContainer);

            _indicator = indicator.GetComponent<HUDIndicator>();
        }
    }
}
