using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Extra
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct PlayerMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            new PlayerMoveJob
            {
                DeltaTime = deltaTime
            }.Schedule();
        }
    }

    [BurstCompile]
    public partial struct PlayerMoveJob : IJobEntity
    {
        public float DeltaTime;
        
        [BurstCompile]
        private void Execute(ref LocalTransform transform, in PlayerMoveInput moveInput, PlayerMoveSpeed moveSpeed)
        {
            transform.Position.xz += moveInput.Value * moveSpeed.Value * DeltaTime;
            if (math.lengthsq(moveInput.Value) > float.Epsilon)
            {
                var forward = new float3(moveInput.Value.x, 0f, moveInput.Value.y);
                transform.Rotation = quaternion.LookRotation(forward, math.up());
            }
        }
    }
}