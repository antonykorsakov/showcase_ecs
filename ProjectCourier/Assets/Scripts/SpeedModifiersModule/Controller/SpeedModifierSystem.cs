using MovementModule.Data;
using SpeedModifiersModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SpeedModifiersModule.Controller
{
    public partial struct SpeedModifierSystem : ISystem
    {
        private const int Min = 0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<MoveSpeedData>()
                .WithAny<SpeedBoostData, SpeedDecayData>()
                .Build(ref state);

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            bool hasChanges = false;

            foreach (var (moveSpeed, entity) in SystemAPI.Query<RefRW<MoveSpeedData>>().WithEntityAccess())
            {
                if (SystemAPI.HasComponent<SpeedBoostData>(entity))
                {
                    var boostData = SystemAPI.GetComponent<SpeedBoostData>(entity);
                    var speed = moveSpeed.ValueRW.Value + boostData.Value;
                    var max = math.max(moveSpeed.ValueRW.Value, boostData.Max);

                    moveSpeed.ValueRW.Value = math.clamp(speed, Min, max);
                    ecb.RemoveComponent<SpeedBoostData>(entity);
                    hasChanges = true;
                }

                if (SystemAPI.HasComponent<SpeedDecayData>(entity))
                {
                    var decayData = SystemAPI.GetComponent<SpeedDecayData>(entity);
                    var speed = moveSpeed.ValueRW.Value - decayData.Value;
                    var max = moveSpeed.ValueRW.Value;

                    moveSpeed.ValueRW.Value = math.clamp(speed, Min, max);
                }
            }

            if (hasChanges)
            {
                ecb.Playback(state.EntityManager);
            }

            ecb.Dispose();
        }
    }
}