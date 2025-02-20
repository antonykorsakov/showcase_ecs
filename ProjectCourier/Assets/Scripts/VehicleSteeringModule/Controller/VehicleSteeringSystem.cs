using Unity.Entities;
using Unity.Mathematics;
using VehicleSteeringModule.Data;

namespace VehicleSteeringModule.Controller
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
                var steeringInput = data.ValueRW.InputState;

                var lowerBound = config.ValueRO.MaxInterval[0];
                var upperBound = config.ValueRO.MaxInterval[1];
                var activeAcceleration = config.ValueRO.ActiveAcceleration;
                var idleDeceleration = config.ValueRO.IdleDeceleration;

                var targetAngle = math.lerp(lowerBound, upperBound, (steeringInput + 1f) / 2f);
                var currentAngle = data.ValueRW.Value;

                if (steeringInput != 0)
                {
                    var angleDelta = activeAcceleration * deltaTime;
                    data.ValueRW.Value = math.lerp(currentAngle, targetAngle, angleDelta);
                }
                else
                {
                    var angleDelta = idleDeceleration * deltaTime;
                    data.ValueRW.Value = math.lerp(currentAngle, 0f, angleDelta);
                }
            }
        }
    }
}