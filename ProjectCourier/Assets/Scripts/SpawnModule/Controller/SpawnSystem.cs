using InputModule.Data;
using InteractableModule.Data;
using SpawnModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SpawnModule.Controller
{
    public partial struct SpawnSystem : ISystem
    {
        private byte _spawnState;
        private float _timer;

        private int _counter1;
        private int _counter2;
        private int _counter3;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnConfigData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Перебираем все сущности, у которых есть компоненты RequestEntityPrefabLoaded
            foreach (var (component, entity) in SystemAPI.Query<RefRO<RequestEntityPrefabLoaded>>().WithEntityAccess())
            {
                // Проверяем, есть ли компонент PrefabLoadResult в этой сущности
                if (!SystemAPI.HasComponent<PrefabLoadResult>(entity))
                    continue;

                var prefabLoadResult = SystemAPI.GetComponent<PrefabLoadResult>(entity);
                var prefab = prefabLoadResult.PrefabRoot;

                // Проверяем, был ли загружен префаб
                if (prefab == Entity.Null)
                {
                    Debug.LogWarning("Prefab is not loaded yet.");
                    continue;
                }

                var config = SystemAPI.GetSingleton<SpawnConfigData>();
                var random = Random.CreateFromIndex((uint)state.GlobalSystemVersion);

                var y = config.SpawnPositionY;

                if (SystemAPI.HasComponent<CharacterSpawnerTag>(entity))
                {
                    if ((_spawnState & 0b001) == 0)
                    {
                        var character = ecb.Instantiate(prefab);
                        var x = random.NextFloat(-5, 5);
                        var z = random.NextFloat(-5, 5);
                        ecb.SetComponent(character, LocalTransform.FromPosition(x, y, z));

                        switch (_counter1)
                        {
                            case 0:
                                ecb.AddComponent<PlayerControlTag>(character);
                                break;

                            case >= 3:
                                _spawnState |= 0b001;
                                break;
                        }

                        _counter1++;
                    }
                }
                else if (SystemAPI.HasComponent<VehicleSpawnerTag>(entity))
                {
                    if ((_spawnState & 0b010) == 0)
                    {
                        var vehicle = ecb.Instantiate(prefab);
                        ecb.SetComponent(vehicle, LocalTransform.FromPosition(-4, y, -4));
                        ecb.AddComponent<InteractableData>(vehicle);

                        switch (_counter2)
                        {
                            case >= 0:
                                _spawnState |= 0b010;
                                break;
                        }

                        _counter2++;
                    }
                }
                else if (SystemAPI.HasComponent<TreeSpawnerTag>(entity))
                {
                    if ((_spawnState & 0b100) == 0)
                    {
                        var tree = ecb.Instantiate(prefab);
                        var x = random.NextFloat(0, 10);
                        var z = random.NextFloat(-5, 10);
                        ecb.SetComponent(tree, LocalTransform.FromPosition(x, y, z));

                        switch (_counter3)
                        {
                            case >= 20:
                                _spawnState |= 0b100;
                                break;
                        }

                        _counter3++;
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            if (_spawnState == 0b111)
            {
                state.Enabled = false;
                // MyDebug(ref state);
            }
        }

        private void MyDebug(ref SystemState state)
        {
            var query = state.EntityManager.CreateEntityQuery(typeof(InteractableData));
            var cars = query.ToEntityArray(Allocator.Temp);
            foreach (var car in cars)
            {
                // Debug.LogError($"Interactable Car: ID = {car.Index}, Version = {car.Version}");
            }

            cars.Dispose();
        }
    }
}