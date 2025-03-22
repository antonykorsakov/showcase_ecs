using PhysicsExpansionModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;

namespace PhysicsExpansionModule.Controller
{
    [BurstCompile]
    public struct CollectTriggerEventsJob : ITriggerEventsJob
    {
        public NativeList<StatefulTriggerEvent> TriggerEvents;
        public void Execute(TriggerEvent triggerEvent) => TriggerEvents.Add(new StatefulTriggerEvent(triggerEvent));
    }
}