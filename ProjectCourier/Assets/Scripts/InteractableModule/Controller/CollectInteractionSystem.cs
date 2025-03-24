using InteractableModule.Data;
using PhysicsExpansionModule.Controller;
using PhysicsExpansionModule.Core;
using PhysicsExpansionModule.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;

namespace InteractableModule.Controller
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
    public partial struct CollectInteractionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InteractableData>();
            state.RequireForUpdate<StatefulTriggerEvent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var (triggerEventBuffer, entity)
                     in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>>().WithEntityAccess())
            {
                for (int i = 0; i < triggerEventBuffer.Length; i++)
                {
                    var triggerEvent = triggerEventBuffer[i];
                    var entityB = triggerEvent.EntityB;
                    if (!SystemAPI.HasComponent<InteractableData>(entityB))
                        continue;

                    switch (triggerEvent.State)
                    {
                        case StatefulEventState.Enter:
                            ecb.SetComponent(entityB, new InteractableData { IsLowDistance = true });
                            break;

                        case StatefulEventState.Exit:
                            ecb.SetComponent(entityB, new InteractableData { IsLowDistance = false });
                            break;
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}