using UnityEngine;
using Zenject;

namespace Runner.Game
{
    public class DemoSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EnemyModel>().FromComponentsInHierarchy().AsCached();
        }
    }
}
