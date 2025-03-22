using PhysicsExpansionModule.Data;
using Unity.Burst;
using Unity.Entities;

namespace PhysicsExpansionModule.Controller
{
    [BurstCompile]
    public partial struct ClearTriggerEventDynamicBufferJob : IJobEntity
    {
        public void Execute(ref DynamicBuffer<StatefulTriggerEvent> eventBuffer) => eventBuffer.Clear();
    }
}