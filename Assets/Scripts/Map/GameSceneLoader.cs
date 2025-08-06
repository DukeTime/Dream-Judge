using DefaultNamespace;
using Map.Player;
using UnityEngine;
using Zenject;


namespace Map
{
    public class GameSceneLoader : MonoInstaller
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private GlobalConfig globalConfig;

        public override void InstallBindings()
        {
            BindConfigs();
            BindPlayer();
        }

        private void BindConfigs()
        {
            Container.Bind<PlayerConfig>().FromInstance(playerConfig);
            Container.Bind<GlobalConfig>().FromInstance(globalConfig);
        }

        private void BindPlayer()
        {
            PlayerController playerController = Container.InstantiatePrefabForComponent<PlayerController>(
                playerPrefab,
                Vector3.zero,
                Quaternion.identity,
                null);
            
            Container.BindInterfacesAndSelfTo<PlayerController>().FromInstance(playerController).AsSingle();
        }
    }
}