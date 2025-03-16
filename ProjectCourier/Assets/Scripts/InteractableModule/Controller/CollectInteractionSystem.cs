using InputModule.Data;
using InteractableModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;

namespace InteractableModule.Controller
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct CollectInteractionSystem : ISystem
    {
        private PhysicsWorld _physicsWorld;
        
        private EntityQuery _playerQuery;
        private EntityQuery _interactableQuery;

        public void OnCreate(ref SystemState state)
        {
            _physicsWorld = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW.PhysicsWorld;
            _playerQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlayerControlTag>(), ComponentType.ReadOnly<LocalTransform>());
            _interactableQuery = state.GetEntityQuery(ComponentType.ReadWrite<InteractableData>(), ComponentType.ReadOnly<LocalTransform>());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (_playerQuery.IsEmpty || _interactableQuery.IsEmpty) 
                return;
            
            var players = _playerQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var interactables = _interactableQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var interactableEntities = _interactableQuery.ToEntityArray(Allocator.Temp);

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // Проход по интерактивным объектам
            for (int i = 0; i < interactables.Length; i++)
            {
                bool canUse = false;
                var interactablePos = interactables[i].Position;
                const float interactionRadius = 2.0f; // Радиус взаимодействия

                // Проверяем всех игроков
                foreach (var player in players)
                {
                    var playerPos = player.Position;
                    if (math.distancesq(playerPos, interactablePos) <= interactionRadius * interactionRadius)
                    {
                        // Проверяем наличие препятствий
                        if (!IsBlockedByObstacle(playerPos, interactablePos, ref _physicsWorld))
                        {
                            canUse = true;
                            break;
                        }
                    }
                }

                // Обновляем компонент
                ecb.SetComponent(interactableEntities[i], new InteractableData { CanUse = canUse });
            }

            ecb.Playback(state.EntityManager);
        }

        private static bool IsBlockedByObstacle(float3 from, float3 to, ref PhysicsWorld physicsWorld)
        {
            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = ~0u,  // Проверяем все слои
                // CollidesWith = (uint)CollisionLayers.Obstacles // Только препятствия
            };

            return physicsWorld.CastRay(new RaycastInput
            {
                Start = from,
                End = to,
                Filter = filter
            }, out _);
        }
    }
}
