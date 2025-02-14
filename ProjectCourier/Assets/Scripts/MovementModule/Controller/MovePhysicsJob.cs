using MovementModule.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace MovementModule.Controller
{
    [BurstCompile]
    public partial struct MovePhysicsJob : IJobEntity
    {
        private void Execute(ref PhysicsVelocity velocity,
            in MoveDirectionData inputData, in MoveSpeedData moveSpeedData)
        {
            if (math.lengthsq(inputData.Value) <= float.Epsilon)
                return;

            velocity.Linear.xz = inputData.Value * moveSpeedData.Value;
        }
    }
}