using PhysicsExpansionModule.Core;
using PhysicsExpansionModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace PhysicsExpansionModule.Controller
{
    // This system converts stream of TriggerEvents to StatefulTriggerEvents that can be stored in a Dynamic Buffer.
    // In order for this conversion, it is required to:
    //    1) Use the 'Raise Trigger Events' option of the 'Collision Response' property on a PhysicsShapeAuthoring component, and
    //    2) Add a StatefulTriggerEventBufferAuthoring component to that entity
    // or, if this is desired on a Character Controller:
    //    1) Tick the 'Raise Trigger Events' flag on the CharacterControllerAuthoring component.
    //       Note: the Character Controller will not become a trigger, it will raise events when overlapping with one
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct StatefulTriggerEventBufferSystem : ISystem
    {
        private StatefulSimulationEventBuffers<StatefulTriggerEvent> m_StateFulEventBuffers;
        private TriggerComponentHandles m_ComponentHandles;
        private EntityQuery m_TriggerEventQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<StatefulTriggerEvent>();

            m_StateFulEventBuffers = new StatefulSimulationEventBuffers<StatefulTriggerEvent>();
            m_StateFulEventBuffers.AllocateBuffers();

            m_TriggerEventQuery = state.GetEntityQuery(builder);
            state.RequireForUpdate(m_TriggerEventQuery);

            m_ComponentHandles = new TriggerComponentHandles(ref state);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            m_StateFulEventBuffers.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_ComponentHandles.Update(ref state);

            state.Dependency = new ClearTriggerEventDynamicBufferJob()
                .ScheduleParallel(m_TriggerEventQuery, state.Dependency);

            m_StateFulEventBuffers.SwapBuffers();

            var currentEvents = m_StateFulEventBuffers.Current;
            var previousEvents = m_StateFulEventBuffers.Previous;

            state.Dependency = new CollectTriggerEventsJob
            {
                TriggerEvents = currentEvents
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

            state.Dependency = new ConvertEventStreamToDynamicBufferJob<StatefulTriggerEvent>
            {
                CurrentEvents = currentEvents,
                PreviousEvents = previousEvents,
                EventLookup = m_ComponentHandles.EventBuffers,
            }.Schedule(state.Dependency);
        }
    }
}