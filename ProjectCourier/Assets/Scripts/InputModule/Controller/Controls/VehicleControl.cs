using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using VehicleModule.Data;

namespace InputModule.Controller.Controls
{
    public struct VehicleControl
    {
        public void HandleInput(ref SystemState state, ref EntityCommandBuffer ecb, Entity vehicleEntity,
            InputAction moveAction)
        {
            float2 inputDirection = moveAction.ReadValue<UnityEngine.Vector2>();

            //
            var xSign = (int)math.sign(inputDirection.x);
            ecb.SetComponent(vehicleEntity, new VehicleSteering { DesiredSteeringAngle = 0.5f * xSign });

            //
            var ySign = (int)math.sign(inputDirection.y);
            var currentSpeed = state.EntityManager.GetComponentData<VehicleSpeedData>(vehicleEntity).CurrentSpeed;
            ecb.SetComponent(vehicleEntity, new VehicleSpeedData
            {
                ThrottleInputState = ySign,
                CurrentSpeed = currentSpeed,
            });
        }
    }
}