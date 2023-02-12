using UnityEngine;
using UnityEngine.UI;

namespace Runner.UI
{
    public class EnemyPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject indicatorPrefab;
        private HUDIndicator _indicator;
        

        private void CreateIndicator(Transform parent)
        {
            GameObject indicator = Instantiate(indicatorPrefab.gameObject, 
                                               Vector3.zero,
                                               Quaternion.identity,
                                               parent);

            _indicator = indicator.GetComponent<HUDIndicator>();
        }
    }
}
