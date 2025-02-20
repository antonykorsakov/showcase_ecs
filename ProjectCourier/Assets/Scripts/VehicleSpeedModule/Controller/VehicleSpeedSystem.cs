using Unity.Entities;
using Unity.Mathematics;
using VehicleModule.Data;
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
                float lowerBound = config.ValueRO.MaxInterval[0];
                float upperBound = config.ValueRO.MaxInterval[1];
                float accelerate;

                switch (data.ValueRW.InputState)
                {
                    case 1:
                        accelerate = config.ValueRO.ActiveAcceleration;
                        break;

                    case -1:
                        accelerate = -config.ValueRO.ActiveAcceleration;
                        break;

                    // Idle
                    default:
                        accelerate = -config.ValueRO.IdleDeceleration;
                        break;
                }

                float value = data.ValueRW.Value + accelerate * deltaTime;
                data.ValueRW.Value = math.clamp(value, lowerBound, upperBound);
            }
        }
    }
}