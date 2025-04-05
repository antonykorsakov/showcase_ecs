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
                    var entityA = triggerEvent.EntityA;
                    var entityB = triggerEvent.EntityB;
                    if (!SystemAPI.HasComponent<InteractableData>(entityB))
                        continue;

                    switch (triggerEvent.State)
                    {
                        case StatefulEventState.Enter:
                        {
                            // var buffer = SystemAPI.GetBuffer<NearbyEntityBufferElement>(entityA);
                            // var isContains = IsContains(buffer, entityB, out _);
                            // if (!isContains)
                            //     ecb.AppendToBuffer(entityA, new NearbyEntityBufferElement { Entity = entityB });
                            break;
                        }

                        case StatefulEventState.Exit:
                        {
                            // var buffer = SystemAPI.GetBuffer<NearbyEntityBufferElement>(entityA);
                            // var isContains = IsContains(buffer, entityB, out int index);
                            // if (isContains)
                            //     buffer.RemoveAt(index);
                            break;
                        }
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        // private bool IsContains(DynamicBuffer<NearbyEntityBufferElement> buffer, Entity entity, out int index)
        // {
        //     for (index = 0; index < buffer.Length; index++)
        //     {
        //         if (buffer[index].Entity == entity)
        //             return true;
        //     }
        //
        //     index = -1;
        //     return false;
        // }
    }
}