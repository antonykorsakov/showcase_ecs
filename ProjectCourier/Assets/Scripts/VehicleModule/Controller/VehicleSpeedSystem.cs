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

            foreach (var (speedData, speedConfig)
                     in SystemAPI.Query<RefRW<VehicleSpeedData>, RefRO<VehicleSpeedConfig>>())
            {
                var maxSpeed = speedConfig.ValueRO.MaxSpeed;
                float accelerate;

                switch (speedData.ValueRW.ThrottleInputState)
                {
                    // Вперёд
                    case 1:
                        accelerate = speedConfig.ValueRO.AccelerationRate;
                        break;

                    // Торможение
                    case -1:
                        accelerate = -speedConfig.ValueRO.AccelerationRate;
                        break;

                    // Бездействие
                    case 0:
                    default:
                        accelerate = -speedConfig.ValueRO.IdleDecelerationRate;
                        break;
                }

                var speed = math.clamp(speedData.ValueRW.CurrentSpeed + accelerate * deltaTime, 0, maxSpeed);
                speedData.ValueRW.CurrentSpeed = speed;
            }
        }
    }
}