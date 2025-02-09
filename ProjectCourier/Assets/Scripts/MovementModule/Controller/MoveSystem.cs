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
            new MoveJob
            {
                DeltaTime = deltaTime
            }.Schedule();
        }
    }
}