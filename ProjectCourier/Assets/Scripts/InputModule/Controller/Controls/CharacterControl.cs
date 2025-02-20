using InputModule.Data;
using MovementModule.Data;
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
                var interactableType = ComponentType.ReadOnly<InteractableTag>();
                var interactableQuery = state.EntityManager.CreateEntityQuery(interactableType);
                var interactableEntities = interactableQuery.ToEntityArray(Allocator.TempJob);

                foreach (var interactEntity in interactableEntities)
                {
                    if (interactEntity == characterEntity)
                        continue;

                    ecb.AddComponent<PlayerControlTag>(interactEntity);
                    ecb.RemoveComponent<PlayerControlTag>(characterEntity);
                    break;
                }

                interactableEntities.Dispose();
            }
        }
    }
}