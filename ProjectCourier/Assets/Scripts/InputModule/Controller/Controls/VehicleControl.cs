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

            var xSign = (int)math.sign(inputDirection.x);
            var ySign = (int)math.sign(inputDirection.y);

            ecb.SetComponent(vehicleEntity, new VehicleSteering { DesiredSteeringAngle = 0.5f * xSign });
            ecb.SetComponent(vehicleEntity, new VehicleSpeed { DesiredSpeed = 3.0f * ySign });
        }
    }
}