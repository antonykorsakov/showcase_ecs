using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace PhysicsExpansionModule.Core
{
    [BurstCompile]
    public struct ConvertEventStreamToDynamicBufferJob<T> : IJob
        where T : unmanaged, IBufferElementData, IStatefulSimulationEvent<T>
    {
        public NativeList<T> PreviousEvents;
        public NativeList<T> CurrentEvents;
        public BufferLookup<T> EventLookup;

        public void Execute()
        {
            var statefulEvents = new NativeList<T>(CurrentEvents.Length, Allocator.Temp);

            StatefulSimulationEventBuffers<T>.GetStatefulEvents(PreviousEvents, CurrentEvents, statefulEvents);

            for (int i = 0; i < statefulEvents.Length; i++)
            {
                var statefulEvent = statefulEvents[i];

                var addToEntityA = EventLookup.HasBuffer(statefulEvent.EntityA);
                var addToEntityB = EventLookup.HasBuffer(statefulEvent.EntityB);

                if (addToEntityA)
                {
                    EventLookup[statefulEvent.EntityA].Add(statefulEvent);
                }

                if (addToEntityB)
                {
                    EventLookup[statefulEvent.EntityB].Add(statefulEvent);
                }
            }
        }
    }
}