using Runner.UI;
using UnityEngine;
using Zenject;

namespace Runner.Game
{
    public class DemoSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerAbilityController>().FromComponentInHierarchy()
                                                  .AsCached()
                                                  .NonLazy();

            Container.Bind<EnemyModel>().FromComponentsInHierarchy()
                                        .AsCached()
                                        .NonLazy();

            Container.Bind<HUDPresenter>().FromComponentInHierarchy()
                                          .AsCached()
                                          .NonLazy();
        }
    }
}
