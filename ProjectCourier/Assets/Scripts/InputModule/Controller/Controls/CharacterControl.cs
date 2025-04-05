using InputModule.Data;
using InteractableModule.Data;
using InventoryModule.Data;
using MovementModule.Data;
using PhysicsExpansionModule.Data;
using SpeedModifiersModule.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputModule.Controller.Controls
{
    public struct CharacterControl
    {
        public void HandleInput(ref SystemState state, ref EntityCommandBuffer ecb, Entity characterEntity,
            InputAction moveAction, InputAction sprintAction, InputAction interactAction)
        {
            if (moveAction.IsPressed())
            {
                ecb.SetComponent(characterEntity, new MoveDirectionData { Value = moveAction.ReadValue<Vector2>() });

                var speedBoostData = sprintAction.IsPressed()
                    ? new SpeedBoostData { Value = 0.1f, Max = 6f }
                    : new SpeedBoostData { Value = 0.05f, Max = 3f };
                ecb.AddComponent(characterEntity, speedBoostData);
            }

            if (interactAction.IsPressed())
            {
                if (state.EntityManager.HasBuffer<StatefulTriggerEvent>(characterEntity))
                {
                    var buffer = state.EntityManager.GetBuffer<StatefulTriggerEvent>(characterEntity);
                    if (buffer.Length > 0)
                    {
                        var collisionEntity = buffer[0].GetOtherEntity(characterEntity);

                        // Проверяем, что это предмет
                        if (state.EntityManager.HasComponent<InteractableData>(collisionEntity))
                        {
                            var query = state.GetEntityQuery(ComponentType.ReadWrite<InventoryData>());
                            if (!query.IsEmptyIgnoreFilter)
                            {
                                var entity = query.GetSingletonEntity();
                                var inventoryData = state.EntityManager.GetComponentData<InventoryData>(entity);

                                // Увеличиваем ресурсы в зависимости от типа источника
                                if (state.EntityManager.HasComponent<WoodSourceTag>(collisionEntity))
                                    inventoryData.Wood++;

                                if (state.EntityManager.HasComponent<StoneSourceTag>(collisionEntity))
                                    inventoryData.Stone++;

                                state.EntityManager.SetComponentData(entity, inventoryData);
                            }
                        }
                    }
                }
            }

            // change CHARACTER to VEHICLE
            // if (interactAction.IsPressed())
            // {
            //     var interactableType = ComponentType.ReadOnly<InteractableTag>();
            //     var interactableQuery = state.EntityManager.CreateEntityQuery(interactableType);
            //     var interactableEntities = interactableQuery.ToEntityArray(Allocator.TempJob);
            //
            //     foreach (var interactEntity in interactableEntities)
            //     {
            //         if (interactEntity == characterEntity)
            //             continue;
            //
            //         ecb.AddComponent<PlayerControlTag>(interactEntity);
            //         ecb.RemoveComponent<PlayerControlTag>(characterEntity);
            //         break;
            //     }
            //
            //     interactableEntities.Dispose();
            // }
        }
    }
}