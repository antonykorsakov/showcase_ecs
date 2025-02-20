using Unity.Entities;
using Unity.Mathematics;
using VehicleModule.Data;

namespace VehicleModule.Controller
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
                var maxSpeed = config.ValueRO.MaxSpeed;
                float accelerate;

                switch (data.ValueRW.InputState)
                {
                    // Вперёд
                    case 1:
                        accelerate = config.ValueRO.AccelerationRate;
                        break;

                    // Торможение
                    case -1:
                        accelerate = -config.ValueRO.AccelerationRate;
                        break;

                    // Бездействие
                    case 0:
                    default:
                        accelerate = -config.ValueRO.IdleDecelerationRate;
                        break;
                }

                var speed = math.clamp(data.ValueRW.Value + accelerate * deltaTime, 0, maxSpeed);
                data.ValueRW.Value = speed;
            }
        }
    }
}