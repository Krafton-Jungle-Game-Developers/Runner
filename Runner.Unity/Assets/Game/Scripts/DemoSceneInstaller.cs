using Runner.UI;
using UnityEngine;
using Zenject;

namespace Runner.Game
{
    public class DemoSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject indicatorPrefab;
        public override void InstallBindings()
        {
            Container.Bind<PlayerEnemyPresenter>().FromComponentInHierarchy()
                                                  .AsCached()
                                                  .NonLazy();

            Container.Bind<EnemyModel>().FromComponentsInHierarchy()
                                        .AsCached()
                                        .NonLazy();

            Container.Bind<HUDPresenter>().FromComponentInHierarchy()
                                          .AsCached()
                                          .NonLazy();

            Container.BindFactory<HUDIndicator, HUDIndicator.Factory>()
                .FromComponentInNewPrefab(indicatorPrefab)
                .UnderTransformGroup("HUD Canvas/Indicator Panel/Element Container");

        }
    }
}
