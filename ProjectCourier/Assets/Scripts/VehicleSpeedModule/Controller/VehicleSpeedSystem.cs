using Unity.Entities;
using Unity.Mathematics;
using VehicleSpeedModule.Data;

namespace VehicleSpeedModule.Controller
{
    public partial struct VehicleSpeedSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<VehicleSpeedData, VehicleSpeedConfig>()
                .Build();

            state.RequireForUpdate(query);
        }

        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (data, config)
                     in SystemAPI.Query<RefRW<VehicleSpeedData>, RefRO<VehicleSpeedConfig>>())
            {
                var lowerBound = config.ValueRO.MaxInterval[0];
                var upperBound = config.ValueRO.MaxInterval[1];

                var accelerate = data.ValueRW.InputState switch
                {
                    > 0 => data.ValueRW.Value >= 0
                        ? config.ValueRO.ActiveAcceleration
                        : config.ValueRO.ActiveDeceleration,

                    < 0 => data.ValueRW.Value <= 0
                        ? -config.ValueRO.ActiveAcceleration
                        : -config.ValueRO.ActiveDeceleration,

                    _ => data.ValueRW.Value >= 0
                        ? -config.ValueRO.IdleDeceleration
                        : config.ValueRO.IdleDeceleration,
                };

                float value = data.ValueRW.Value + accelerate * deltaTime;
                value = data.ValueRW.Value switch
                {
                    > 0.01f => math.clamp(value, 0, upperBound),
                    < -0.01f => math.clamp(value, lowerBound, 0),
                    _ => math.clamp(value, lowerBound, upperBound),
                };

                if (math.abs(value) < 0.001f)
                    value = 0f;

                data.ValueRW.Value = value;
            }
        }
    }
}