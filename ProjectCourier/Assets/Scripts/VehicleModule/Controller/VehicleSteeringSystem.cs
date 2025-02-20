using Unity.Entities;
using Unity.Mathematics;
using VehicleModule.Data;

namespace VehicleModule.Controller
{
    public partial struct VehicleSteeringSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<VehicleSteeringData, VehicleSteeringConfig>()
                .Build();

            state.RequireForUpdate(query);
        }

        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (data, config)
                     in SystemAPI.Query<RefRW<VehicleSteeringData>, RefRO<VehicleSteeringConfig>>())
            {
                float steeringInput = data.ValueRW.InputState;

                float maxSteeringAngle = config.ValueRO.MaxSteeringAngle;
                float steeringSpeed = config.ValueRO.SteeringSpeed;
                float returnSpeed = config.ValueRO.ReturnSpeed;

                float targetAngle = steeringInput * maxSteeringAngle;
                float currentAngle = data.ValueRW.Value;
                float angleDelta = (steeringInput != 0 ? steeringSpeed : returnSpeed) * deltaTime;

                data.ValueRW.Value = math.lerp(currentAngle, targetAngle, angleDelta);
            }
        }
    }
}