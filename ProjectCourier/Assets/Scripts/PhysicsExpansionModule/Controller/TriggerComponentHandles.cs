using PhysicsExpansionModule.Data;
using Unity.Entities;

namespace PhysicsExpansionModule.Controller
{
    public struct TriggerComponentHandles
    {
        public BufferLookup<StatefulTriggerEvent> EventBuffers;

        public TriggerComponentHandles(ref SystemState systemState)
        {
            EventBuffers = systemState.GetBufferLookup<StatefulTriggerEvent>();
        }

        public void Update(ref SystemState systemState)
        {
            EventBuffers.Update(ref systemState);
        }
    }
}