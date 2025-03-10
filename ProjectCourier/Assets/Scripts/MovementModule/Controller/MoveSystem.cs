using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace MovementModule.Controller
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            // without physics
            {
                new MoveJob
                {
                    DeltaTime = deltaTime
                }.Schedule();
            }

            // with physics
            if (false)
            {
                new MovePhysicsJob().Schedule();
            }
        }
    }
}