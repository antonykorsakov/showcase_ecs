using MovementModule.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MovementModule.Controller
{
    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref LocalTransform transform, in MoveDirectionData inputData, MoveSpeedData moveSpeedData)
        {
            if (math.lengthsq(inputData.Value) <= float.Epsilon)
                return;

            transform.Position.xz += inputData.Value * moveSpeedData.Value * DeltaTime;
            var forward = new float3(inputData.Value.x, 0f, inputData.Value.y);
            transform.Rotation = quaternion.LookRotation(forward, math.up());
        }
    }
}