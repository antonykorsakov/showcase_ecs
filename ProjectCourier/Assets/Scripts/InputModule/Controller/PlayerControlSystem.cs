using InputModule.Controller.Controls;
using InputModule.Data;
using MovementModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.InputSystem;
using VehicleModule.Data;

namespace InputModule.Controller
{
    [BurstCompile]
    public partial struct PlayerControlSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<InputTag>(out Entity entity))
                return;

            var inputActionAsset = state.EntityManager.GetComponentObject<InputActionAsset>(entity);
            if (inputActionAsset == null)
                return;

            InputAction moveAction = EnableAction(inputActionAsset, "Move");
            InputAction sprintAction = EnableAction(inputActionAsset, "Sprint");
            InputAction interactAction = EnableAction(inputActionAsset, "Interact");
            InputAction quitAction = EnableAction(inputActionAsset, "Quit");

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            foreach (var (_, playerEntity) in SystemAPI.Query<RefRO<PlayerControlTag>>().WithEntityAccess())
            {
                // Character control
                if (SystemAPI.HasComponent<MoveDirectionData>(playerEntity))
                {
                    new CharacterControl().HandleInput(ref state, ref ecb, playerEntity,
                        moveAction, sprintAction, interactAction);
                }
                // Vehicle control
                else if (SystemAPI.HasComponent<VehicleData>(playerEntity))
                {
                    new VehicleControl().HandleInput(ref state, ref ecb, playerEntity,
                        moveAction);
                }

                // Application control
                new AppControl().HandleInput(ref state, ref ecb, quitAction);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private InputAction EnableAction(InputActionAsset inputActionAsset, string actionName)
        {
            var action = inputActionAsset.FindAction(actionName);
            if (action is { enabled: false })
                action.Enable();

            return action;
        }
    }
}