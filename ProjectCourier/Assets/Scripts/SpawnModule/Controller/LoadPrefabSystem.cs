using SpawnModule.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Scenes;

namespace SpawnModule.Controller
{
    public partial struct LoadPrefabSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnConfigData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var spawnConfigData = SystemAPI.GetSingleton<SpawnConfigData>();

            var characterSpawnEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<CharacterSpawnerTag>(characterSpawnEntity);
            state.EntityManager.AddComponentData(characterSpawnEntity, new RequestEntityPrefabLoaded
            {
                Prefab = spawnConfigData.CharacterPrefab
            });

            var vehicleSpawnEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<VehicleSpawnerTag>(vehicleSpawnEntity);
            state.EntityManager.AddComponentData(vehicleSpawnEntity, new RequestEntityPrefabLoaded
            {
                Prefab = spawnConfigData.CarPrefab
            });

            var treeSpawnEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<TreeSpawnerTag>(treeSpawnEntity);
            state.EntityManager.AddComponentData(treeSpawnEntity, new RequestEntityPrefabLoaded
            {
                Prefab = spawnConfigData.TreePrefab
            });
        }
    }
}