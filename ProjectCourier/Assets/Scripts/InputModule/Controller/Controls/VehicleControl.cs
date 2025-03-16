using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using VehicleModule.Data;
using VehicleSpeedModule.Data;
using VehicleSteeringModule.Data;

namespace InputModule.Controller.Controls
{
    public struct VehicleControl
    {
        public void HandleInput(ref SystemState state, ref EntityCommandBuffer ecb, Entity vehicleEntity,
            InputAction moveAction)
        {
            float2 inputDirection = moveAction.ReadValue<UnityEngine.Vector2>();

            // set speed
            {
                var ySign = (int)math.sign(inputDirection.y);
                var value = state.EntityManager.GetComponentData<VehicleSpeedData>(vehicleEntity).Value;
                ecb.SetComponent(vehicleEntity, new VehicleSpeedData
                {
                    InputState = ySign,
                    Value = value,
                });
            }

            // set steering
            {
                var xSign = (int)math.sign(inputDirection.x);
                var value = state.EntityManager.GetComponentData<VehicleSteeringData>(vehicleEntity).Value;
                ecb.SetComponent(vehicleEntity, new VehicleSteeringData
                {
                    InputState = xSign,
                    Value = value,
                });
            }
        }
    }
}