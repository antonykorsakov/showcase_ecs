using PhysicsExpansionModule.Controller;
using PhysicsExpansionModule.Core;
using PhysicsExpansionModule.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace InteractableModule.Controller
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
    public partial struct CollectInteractionSystem : ISystem
    {
        // [BurstCompile]
        // public void OnCreate(ref SystemState state)
        // {
        //     // state.RequireForUpdate<SimulationSingleton>();
        // }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var sim = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();

            foreach (var (triggerEventBuffer, entity)
                     in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>>().WithEntityAccess())
            {
                for (int i = 0; i < triggerEventBuffer.Length; i++)
                {
                    var triggerEvent = triggerEventBuffer[i];
                    var otherEntity = triggerEvent.GetOtherEntity(entity);

                    switch (triggerEvent.State)
                    {
                        case StatefulEventState.Enter:
                        case StatefulEventState.Stay:
                        case StatefulEventState.Exit:
                            Debug.Log($"{triggerEvent.State} Trigger; " +
                                      $"EntityA.index = {triggerEvent.EntityA.Index}; " +
                                      $"BodyIndexA = {triggerEvent.BodyIndexA}; " +
                                      $"EntityB.index = {triggerEvent.EntityB.Index}; " +
                                      $"BodyIndexB = {triggerEvent.BodyIndexB};");
                            break;
                    }
                }
            }
        }
    }
}